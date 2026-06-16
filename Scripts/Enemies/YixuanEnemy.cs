using STS2RitsuLib.Interop.AutoRegistration;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Enemies
{
    internal class YixuanEnemy : ModMonsterTemplate
    {
        public YixuanEnemy()
        {
            // 手动注册场景转换，这样你就不用给 .tscn 挂脚本了
            CustomVisualsPath?.RegisterSceneForConversion<NCreatureVisuals>();
        }

        // 根据进阶提高最小血量，进阶8及以上为225，否则为200
        public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 225, 200);

        // 根据进阶提高最大血量，进阶8及以上为250，否则为220
        public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 250, 220);

        public override string? CustomVisualsPath => "res://scenes/_Yixuan/yixuan_char.tscn";

        public override async Task AfterAddedToRoom()
        {
            await PowerCmd.Apply<YixuanEnemyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        }

        private int AAD_Damage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);

        private int MultiHitDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 2);

        private bool isjustMultihit = false;

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            List<MonsterState> list = new List<MonsterState>();

            var StartMove = new MoveState(
                "START_MOVE", // 状态ID
                StartMoveF, // 执行函数，或者直接用lambda也可
                                 // 以下是可变参数，可以填写任意数量的意图，全部展示
                new BuffIntent()
            );

            var AttactAndDefenceMove = new MoveState(
                "ATTACK_AND_DEFENCE", // 状态ID
                AttackAndDefenceF, // 执行函数，或者直接用lambda也可
                            // 以下是可变参数，可以填写任意数量的意图，全部展示
                new SingleAttackIntent(AAD_Damage),
                new DefendIntent()
            );

            var HeavyAttack = new MoveState(
                "HEAVY_ATTACK", // 状态ID
                HeavyAttackF,
                new MultiAttackIntent(MultiHitDamage, 8)
            );

            var PowerUp = new MoveState(
                "POWER_UP",
                PowerUpF,
                new BuffIntent()
             );

            var DebuffAdd = new MoveState(
                "DEBUFF_ADD",
                DebuffAddF,
                new DebuffIntent()
            );

            var UnclearMove = new MoveState(
                "UNCLEAR_MOVE",
                UnclearMoveF,
                new UnknownIntent()
            );

            // 设置状态转换，意图1后接意图2，意图2后接意图1
            StartMove.FollowUpState = UnclearMove;
            UnclearMove.FollowUpState = UnclearMove;
            HeavyAttack.FollowUpState = UnclearMove;
            PowerUp.FollowUpState = UnclearMove;
            DebuffAdd.FollowUpState = UnclearMove;

            list.Add( StartMove );
            list.Add( AttactAndDefenceMove );
            list.Add( HeavyAttack );
            list.Add( PowerUp );
            list.Add( DebuffAdd );
            list.Add( UnclearMove );

            return new MonsterMoveStateMachine(list, StartMove);
        }

        private async Task StartMoveF(IReadOnlyList<Creature> targets)
        {
            //TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-YIXUAN_ENEMY.moves.START_MOVE.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), base.Creature, base.Creature.MaxHp * 0.1m, base.Creature, null);
            if(MiyabiModConfig.MiyabiEnemiesStronger)
                await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        }

        private async Task AttackAndDefenceF(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                .Attack(MiyabiModConfig.MiyabiEnemiesStronger ? 15 : AAD_Damage)
                .FromMonster(this)
                // .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
                .WithAttackerFx(null, AttackSfx) // 攻击音效
                .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
                .Execute(null);

            decimal block = MiyabiModConfig.MiyabiEnemiesStronger ? 20 : 15;
            await CreatureCmd.GainBlock(Creature, block, ValueProp.Move, null);
        }

        private async Task HeavyAttackF(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(MiyabiModConfig.MiyabiEnemiesStronger ? 5 : MultiHitDamage)
                    .WithHitCount(8)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);

            isjustMultihit = true;
        }

        private async Task PowerUpF(IReadOnlyList<Creature> targets)
        {
            //TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-YIXUAN_ENEMY.moves.POWER_UP.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
            await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);

            await CreatureCmd.GainBlock(Creature, MiyabiModConfig.MiyabiEnemiesStronger ? 30m : 20m, ValueProp.Move, null);
        }

        private async Task DebuffAddF(IReadOnlyList<Creature> targets)
        {
            foreach (Creature creature in targets)
            {
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), creature, 2m, base.Creature, null);
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), creature, 2m, base.Creature, null);
            }
            await CreatureCmd.GainBlock(Creature, MiyabiModConfig.MiyabiEnemiesStronger ? 30m : 20m, ValueProp.Move, null);
        }

        private async Task UnclearMoveF(IReadOnlyList<Creature> targets)
        {
            Creature creature = targets.OrderBy(t => t.CurrentHp).FirstOrDefault();
            //{ 
                if(!isjustMultihit 
                    && (creature.Block <= 8 * MultiHitDamage - 8 
                    || (creature.HasPower<VulnerablePower>() 
                    && creature.Block <= 8 * MultiHitDamage * 1.5 - 8)))
                {
                    await HeavyAttackF(targets);
                    return;
                }

                if (creature.HasPower<VulnerablePower>())
                {
                    int re = MiyabiFuncBase.RandomInt(0, 3, CombatState.Players.OrderBy(p => p.NetId).FirstOrDefault());
                    if(re != 0)
                        await PowerUpF(targets);
                    else
                        await AttackAndDefenceF(targets);

                    isjustMultihit = false;
                    return;
                }
                else
                {
                    await DebuffAddF(targets);
                    isjustMultihit = false;
                    return;
                }
            //}

            isjustMultihit = false;
            await CreatureCmd.GainBlock(Creature, 60m, ValueProp.Move, null);
        }
    }
}
