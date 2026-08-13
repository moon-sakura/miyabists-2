using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Miyabists2.Scripts.Events;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Enemies
{
    [RegisterMonster]
    internal class OldHuntingDogEnemy : ModMonsterTemplate
    {
        public OldHuntingDogEnemy()
        {
            // 手动注册场景转换，这样你就不用给 .tscn 挂脚本了
            CustomVisualsPath?.RegisterSceneForConversion<NCreatureVisuals>();
        }

        // 旧日猎犬·乌利迪姆
        // 注意：CreateVisuals 会用 GetScene().Instantiate<NCreatureVisuals>() 加载，必须是 .tscn 场景而非贴图。
        // 场景里通过 Sprite2D 引用 images/elseui/dogEnemy.png；构造函数里的 RegisterSceneForConversion 负责转换。
        public override string? CustomVisualsPath => "res://scenes/monsters/dog_enemy.tscn";

        // 本次战斗是第几次遇到猎犬（由迷宫诡域事件在进入战斗前设置，默认1=无强化）
        public int NextEncounterIndex = 1;

        // 每次遇到，最大生命值提升（遇到次数-1）*10%
        private decimal HpMultiplier => 1m + Math.Max(0, NextEncounterIndex - 1) * 0.1m;

        // 根据进阶提高最小血量，进阶8及以上为155，否则为125；再按遇到次数提升
        public override int MinInitialHp => (int)(AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 155, 125) * HpMultiplier);

        // 根据进阶提高最大血量，进阶8及以上为175，否则为145；再按遇到次数提升
        public override int MaxInitialHp => (int)(AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 175, 145) * HpMultiplier);

        // 偷窃层数：10 + 5 *（遇到次数-1）
        private int StealAmount => 10 + 5 * Math.Max(0, NextEncounterIndex - 1);

        // 战斗开始时，根据遇到次数给予猎犬buff：无实体、力量；以及游戏内已有的「偷窃」能力
        public override async Task AfterAddedToRoom()
        {
            int stacks = Math.Max(0, NextEncounterIndex - 1);
            var ctx = new ThrowingPlayerChoiceContext();
            if (stacks > 0)
            {
                await PowerCmd.Apply<IntangiblePower>(ctx, base.Creature, stacks, base.Creature, null);
                await PowerCmd.Apply<StrengthPower>(ctx, base.Creature, stacks, base.Creature, null);
            }
            // 偷窃能力不会自动扣钱：Steal() 里要求 Target 指向玩家，否则 Target 为 null 直接返回。
            // 照抄 Gremlin Merc 的做法：给每个玩家单独建一个 ThieveryPower 实例、手动设置 Target，
            // 再挂到猎犬身上；攻击动作里再显式调用 Steal() 才真正扣钱。
            foreach (Player player in base.CombatState.Players)
            {
                ThieveryPower thieveryPower = (ThieveryPower)ModelDb.Power<ThieveryPower>().ToMutable();
                thieveryPower.Target = player.Creature;
                await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), thieveryPower, base.Creature, StealAmount, base.Creature, null);
            }

            await CreatureCmd.SetMaxAndCurrentHp(base.Creature, base.Creature.MaxHp * HpMultiplier);
        }

        // ===== 动作 =====

        // 偷钱并挂「赃物袋」：每次实际偷到的钱记为一次 HeistPower，猎犬被击杀时按这些金额返给玩家。
        // 注意三点：
        // 1) GetPowerInstances 是 _powers 的实时枚举，循环里再 Apply 会改集合 → 必须先 ToList() 快照。
        // 2) HeistPower 和 ThieveryPower 一样，Target 要手动指向被偷的玩家，否则 BeforeDeath 里 base.Target.Player 空引用。
        // 3) 金额用本次实际偷到数（偷前偷后 DynamicVars.Gold 之差），不要用层数 Amount——
        //    玩家钱不够时 Steal() 会按 Math.Min 截断，按层数返还会多退钱。
        private async Task Steal()
        {
            foreach (ThieveryPower powerInstance in base.Creature.GetPowerInstances<ThieveryPower>().ToList())
            {
                int before = powerInstance.DynamicVars.Gold.IntValue;
                await powerInstance.Steal();
                int stolen = powerInstance.DynamicVars.Gold.IntValue - before;
                if (stolen <= 0)
                {
                    continue;
                }
                HeistPower heistPower = (HeistPower)ModelDb.Power<HeistPower>().ToMutable();
                heistPower.Target = powerInstance.Target;
                await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), heistPower, base.Creature, stolen, base.Creature, null);
            }
        }

        // 撕咬：12-15伤害 + 3层易伤（偷钱由偷窃能力处理）
        private int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);
        private async Task BiteMove(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                .Attack(BiteDamage)
                .FromMonster(this)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(null);
            foreach (Creature creature in targets)
            {
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), creature, 3m, base.Creature, null);
            }

            await Steal();
        }

        // 穿刺攻击：10-14伤害 + 3层脆弱（偷钱由偷窃能力处理）
        private int PierceDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 10);
        private async Task PierceMove(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                .Attack(PierceDamage)
                .FromMonster(this)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(null);
            foreach (Creature creature in targets)
            {
                await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), creature, 3m, base.Creature, null);
            }
            await Steal();
        }

        // 狗车：28-35伤害
        private int DogCartDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 35, 28);
        private async Task DogCartMove(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                .Attack(DogCartDamage)
                .FromMonster(this)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(null);

            await Steal();
        }

        // 射击：14-18伤害 + 6层消亡
        private int ShootDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 14);
        private async Task ShootMove(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                .Attack(ShootDamage)
                .FromMonster(this)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(null);
            foreach (Creature creature in targets)
            {
                await PowerCmd.Apply<DisintegrationPower>(new ThrowingPlayerChoiceContext(), creature, 6m, base.Creature, null);
            }


            await Steal();
        }

        // 潜伏蓄力：获得6点力量
        private async Task CrouchMove(IReadOnlyList<Creature> targets)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, NextEncounterIndex, base.Creature, null);
        }

        // 逃跑：主动离开战斗，不视为击杀、不给遗骸遗物，正常回到事件
        private async Task FleeMove(IReadOnlyList<Creature> targets)
        {
            if (base.CombatState.Encounter is UridimuHoundEncounter battlewornDummyEventEncounter)
            {
                battlewornDummyEventEncounter.RanOutOfTime = true;
            }
            await CreatureCmd.Escape(Creature, true);
        }

        // ===== 逃跑条件：回合开始时已累计3回合没有无实体 → 本回合行为变为逃跑 =====
        private int _turnsWithoutIntangible = 0;
        public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if(side != Creature.Side)
            {
                return base.AfterSideTurnEnd(choiceContext, side, participants);
            }
            if (!Creature.HasPower<IntangiblePower>())
            {
                _turnsWithoutIntangible++;
            }
            return base.AfterSideTurnEnd(choiceContext, side, participants);
        }
        private bool ShouldFlee()
        {
            return _turnsWithoutIntangible >= 3;
        }

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            List<MonsterState> list = new List<MonsterState>();

            // 撕咬：攻击 + 易伤
            var bite = new MoveState(
                "BITE",
                BiteMove,
                new SingleAttackIntent(BiteDamage),
                new DebuffIntent()
            );

            // 穿刺攻击：攻击 + 脆弱
            var pierce = new MoveState(
                "PIERCE",
                PierceMove,
                new SingleAttackIntent(PierceDamage),
                new DebuffIntent()
            );

            // 狗车：重击
            var dogCart = new MoveState(
                "DOG_CART",
                DogCartMove,
                new SingleAttackIntent(DogCartDamage)
            );

            // 射击：攻击 + 消亡
            var shoot = new MoveState(
                "SHOOT",
                ShootMove,
                new SingleAttackIntent(ShootDamage),
                new DebuffIntent()
            );

            // 潜伏蓄力：获得力量
            var crouch = new MoveState(
                "CROUCH",
                CrouchMove,
                new BuffIntent()
            );

            // 逃跑
            var flee = new MoveState(
                "FLEE",
                FleeMove,
                new EscapeIntent()
            );

            // 5种动作随机使用（互不连用）
            RandomBranchState randomBranch = new RandomBranchState("RAND");
            randomBranch.AddBranch(bite, MoveRepeatType.CannotRepeat);
            randomBranch.AddBranch(pierce, MoveRepeatType.CannotRepeat);
            randomBranch.AddBranch(dogCart, MoveRepeatType.CannotRepeat);
            randomBranch.AddBranch(shoot, MoveRepeatType.CannotRepeat);
            randomBranch.AddBranch(crouch, MoveRepeatType.CannotRepeat);

            // 条件分支（回合开始选择动作时判断）：
            // 已累计4回合没有无实体 → 本回合逃跑；否则 → 随机分支
            ConditionalBranchState fleeCheck = new ConditionalBranchState("FLEE_CHECK");
            fleeCheck.AddState(flee, ShouldFlee);
            fleeCheck.AddState(randomBranch, () => true);

            foreach (MoveState state in new[] { bite, pierce, dogCart, shoot, crouch, flee })
            {
                state.FollowUpState = fleeCheck;
            }

            list.Add(bite);
            list.Add(pierce);
            list.Add(dogCart);
            list.Add(shoot);
            list.Add(crouch);
            list.Add(flee);
            list.Add(randomBranch);
            list.Add(fleeCheck);

            // 初始动作为潜伏蓄力
            return new MonsterMoveStateMachine(list, crouch);
        }
    }
}
