using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Enemies
{
    [RegisterMonster]
    internal class MiyabiBoss : ModMonsterTemplate
    {
        public MiyabiBoss()
        {
            // 手动注册场景转换，这样你就不用给 .tscn 挂脚本了
            CustomVisualsPath?.RegisterSceneForConversion<NCreatureVisuals>();
        }
        // 根据进阶提高最小血量
        public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 根据进阶提高最大血量
        public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 轻击伤害
        private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);
        private int BasicBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 15);

        // 重击伤害
        private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 35, 30);

        // 多段攻击的数值
        private int MultiHitDamage => 5;
        private int MultiHitCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 4);

        // 统一意图与实际伤害的有效数值
        private int EffectiveBasicDamage => MiyabiModConfig.MiyabiEnemiesStronger ? 18 : BasicDamage;
        private int EffectiveHeavyDamage => MiyabiModConfig.MiyabiEnemiesStronger ? 42 : HeavyDamage;
        private int EffectiveMultiHitDamage => MiyabiModConfig.MiyabiEnemiesStronger ? 6 : MultiHitDamage;

        // 怪物场景，如果你的场景没有挂载脚本，参考这个
        //public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://test/scenes/test_monster.tscn");

        // 如果你挂载了自己的自定义脚本，使用这个
        public override string? CustomVisualsPath => "res://scenes/miyabi_char.tscn";


        // 战斗开始时，在这里给自己上buff之类
        public override async Task AfterAddedToRoom()
        {
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 10m * CombatState.Enemies.Count, base.Creature, null);
            await PowerCmd.Apply<MiyabiBossPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        }

        private bool HeavyAttackUsed = false;
        private bool MultiHitUsed = false;

        /// <summary>
        /// 检查是否有任何玩家处于"易伤"状态：
        /// 拥有 BreakPlayerPower，或抽牌堆中攻击牌不足且无减伤能力
        /// </summary>
        private bool IsAnyEnemyVulnerable()
        {
            foreach (var creature in base.Creature.CombatState.Enemies)
            {
                if (creature.IsPlayer)
                {
                    if (creature.HasPower<BreakPlayerPower>())
                        return true;
                    if (creature.Player.PlayerCombatState.DrawPile.Cards.CountBy(c => !c.GainsBlock).Count() < 3
                        && !creature.HasPower<IntangiblePower>())
                        return true;
                }
                if (creature.IsMonster)
                {
                    if (creature.HasPower<BreakPower>())
                        return true;
                    if (creature.HasPower<VulnerablePower>()
                        && !creature.HasPower<IntangiblePower>())
                        return true;
                }
            }
            return false;
        }

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            List<MonsterState> list = new List<MonsterState>();

            // 意图1：造成伤害，获得格挡
            var basicAttack = new MoveState(
                "BASIC_MOVE", // 状态ID
                BasicMove, // 执行函数，或者直接用lambda也可
                           // 以下是可变参数，可以填写任意数量的意图，全部展示
                new SingleAttackIntent(EffectiveBasicDamage),
                new DefendIntent()
            );

            // 意图2：重击
            var heavyAttack = new MoveState(
                "EJIZHAN",
                EJiZhan,
                new SingleAttackIntent(EffectiveHeavyDamage)
            );

            var multiHitAttack = new MoveState(
                "MINGCANXUE",
                MingCanXue,
                new MultiAttackIntent(EffectiveMultiHitDamage, MultiHitCount)
            );

            var powerUp = new MoveState(
                "POWER_UP",
                PowerUp,
                new BuffIntent(),
                new DefendIntent()
            );

            var dashAttack = new MoveState(
                "DASH_ATTACK",
                DashAttackF,
                new SingleAttackIntent(EffectiveBasicDamage),
                new BuffIntent()
            );



            RandomBranchState randomBranchState = new RandomBranchState("ATK");
            randomBranchState.AddBranch(multiHitAttack, MoveRepeatType.CannotRepeat);
            randomBranchState.AddBranch(heavyAttack, MoveRepeatType.CannotRepeat);

            RandomBranchState randomBranchState2 = new RandomBranchState("SLIP");
            randomBranchState2.AddBranch(powerUp, MoveRepeatType.CannotRepeat);
            randomBranchState2.AddBranch(dashAttack, MoveRepeatType.CannotRepeat);

            ConditionalBranchState conditionalBranchState = new ConditionalBranchState("COND");

            // 优先级1：Boss 失去 Slippery 时，立即补回（生存优先）
            conditionalBranchState.AddState(randomBranchState2,
                () =>
                {
                    if (!Creature.HasPower<SlipperyPower>())
                    {
                        HeavyAttackUsed = false;
                        MultiHitUsed = false;
                        return true;
                    }
                    return false;
                });

            // 优先级2：有易伤玩家时，交替使用重击/多段惩罚
            conditionalBranchState.AddState(heavyAttack,
                () =>
                {
                    if (!HeavyAttackUsed && IsAnyEnemyVulnerable())
                    {
                        HeavyAttackUsed = true;
                        MultiHitUsed = false;
                        return true;
                    }
                    return false;
                });

            conditionalBranchState.AddState(multiHitAttack,
                () =>
                {
                    if (!MultiHitUsed && IsAnyEnemyVulnerable())
                    {
                        MultiHitUsed = true;
                        HeavyAttackUsed = false;
                        return true;
                    }
                    return false;
                });

            // 优先级3：兜底 — 强化自身
            conditionalBranchState.AddState(powerUp, () =>
            {
                HeavyAttackUsed = false;
                MultiHitUsed = false;
                return true;
            });

            // 设置状态转换，意图1后接意图2，意图2后接意图1
            basicAttack.FollowUpState = randomBranchState;
            multiHitAttack.FollowUpState = conditionalBranchState;
            heavyAttack.FollowUpState = conditionalBranchState;
            powerUp.FollowUpState = randomBranchState;
            dashAttack.FollowUpState = randomBranchState;

            list.Add(basicAttack);
            list.Add(multiHitAttack);
            list.Add(heavyAttack);
            list.Add(powerUp);
            list.Add(randomBranchState);
            list.Add(dashAttack);
            list.Add(conditionalBranchState);
            list.Add(randomBranchState2);

            // 添加2个意图，并且初始意图设成 basicAttack
            return new MonsterMoveStateMachine(list, basicAttack);
        }


        // 意图1执行实际效果
        private async Task BasicMove(IReadOnlyList<Creature> targets)
        {
            // 说话
            //TalkCmd.Play(L10NMonsterLookup("TEST-TEST_MONSTER.moves.BASIC_ATTACK.banter"), Creature, VfxColor.Blue);
            await DamageCmd
                .Attack(EffectiveBasicDamage)
                .FromMonster(this)
                // .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
                .WithAttackerFx(null, AttackSfx) // 攻击音效
                .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
                .Execute(null);
            await CreatureCmd.GainBlock(Creature, MiyabiModConfig.MiyabiEnemiesStronger ? 25m : BasicBlock, ValueProp.Move, null);
        }

        private async Task PowerUp(IReadOnlyList<Creature> targets)
        {
            //TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-MIYABI_BOSS.moves.POWER_UP.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);

            decimal slipperyAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 6m : 4m;
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, slipperyAmount * CombatState.Enemies.Count, base.Creature, null);

            decimal thornsAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 3m : 2m;
            await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, thornsAmount, base.Creature, null);

            decimal debuffAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 3m : 2m;
            foreach (var creature in CombatState.Enemies)
            {
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), creature, debuffAmount, base.Creature, null);
                await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), creature, debuffAmount, base.Creature, null);
            }

        }

        private async Task EJiZhan(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(EffectiveHeavyDamage)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);
        }

        private async Task MingCanXue(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(EffectiveMultiHitDamage)
                    .WithHitCount(MultiHitCount)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);
        }

        private async Task DashAttackF(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(BasicDamage)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);

            decimal slipperyAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 6m : 4m;
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, slipperyAmount * CombatState.Enemies.Count, base.Creature, null);
        }
    }
}
