using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
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
    internal class MiyabiGhostEnemy : CustomMonsterModel
    {
        // 根据进阶提高最小血量，进阶8及以上为120，否则为100
        public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 140);

        // 根据进阶提高最大血量，进阶8及以上为140，否则为120
        public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 150);

        // 意图1的数值，伤害和格挡，根据进阶提高伤害
        private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);
        private int BasicBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 10);

        // 意图2的数值，重击伤害，根据进阶提高伤害
        private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 25);

        // 多段攻击的数值
        private int MultiHitDamage => 4;
        private int MultiHitCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 4);

        // 怪物场景，如果你的场景没有挂载脚本，参考这个
        //public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://test/scenes/test_monster.tscn");

        // 如果你挂载了自己的自定义脚本，使用这个
        public override string? CustomVisualPath => "res://scenes/miyabi_char.tscn";


        // 战斗开始时，在这里给自己上buff之类
        public override async Task AfterAddedToRoom()
        {
            decimal slipperyAmount = MiyabiModConfig.MiyabiEnemiesStronger?6m:4m;
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, slipperyAmount*CombatState.Players.Count, base.Creature, null);
            //await PowerCmd.Apply<MiyabiBossPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1, base.Creature, null);
        }

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            List<MonsterState> list = new List<MonsterState>();

            // 意图1：造成伤害，获得格挡
            var basicAttack = new MoveState(
                "BASIC_ATTACK", // 状态ID
                BasicAttackMove, // 执行函数，或者直接用lambda也可
                                 // 以下是可变参数，可以填写任意数量的意图，全部展示
                new SingleAttackIntent(BasicDamage),
                new DefendIntent()
            );

            // 意图2：重击
            var heavyAttack = new MoveState(
                "HEAVY_ATTACK",
                async targets => await DamageCmd // 意图2实际执行效果，这里直接用lambda
                    .Attack(MiyabiModConfig.MiyabiEnemiesStronger ? 36m : HeavyDamage)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null),
                new SingleAttackIntent(HeavyDamage)
            );

            var multiHitAttack = new MoveState(
                "MULTIHIT_ATTACK",
                async targets => await DamageCmd
                    .Attack(MiyabiModConfig.MiyabiEnemiesStronger ? 5m : MultiHitDamage)
                    .WithHitCount(MultiHitCount)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null),
                new MultiAttackIntent(MultiHitDamage, MultiHitCount)
            );

            var powerUp = new MoveState(
                "POWER_UP",
                PowerUp,
                new BuffIntent()
            );

            RandomBranchState randomBranchState = new RandomBranchState("RAND");
            randomBranchState.AddBranch(multiHitAttack, MoveRepeatType.CannotRepeat);
            randomBranchState.AddBranch(heavyAttack, MoveRepeatType.CannotRepeat);

            // 或者你也可以创建RandomBranchState（随机意图分支）和ConditionalBranchState（条件意图分支）来实现更复杂的状态转换逻辑

            // 设置状态转换，意图1后接意图2，意图2后接意图1
            basicAttack.FollowUpState = randomBranchState;
            multiHitAttack.FollowUpState = powerUp;
            heavyAttack.FollowUpState = powerUp;
            powerUp.FollowUpState = basicAttack;

            list.Add(basicAttack);
            list.Add(multiHitAttack);
            list.Add(heavyAttack);
            list.Add(powerUp);
            list.Add(randomBranchState);

            // 添加2个意图，并且初始意图设成 basicAttack
            return new MonsterMoveStateMachine(list, basicAttack);
        }

        // 意图1执行实际效果
        private async Task BasicAttackMove(IReadOnlyList<Creature> targets)
        {
            // 说话
            //TalkCmd.Play(L10NMonsterLookup("TEST-TEST_MONSTER.moves.BASIC_ATTACK.banter"), Creature, VfxColor.Blue);
            await DamageCmd
                .Attack(MiyabiModConfig.MiyabiEnemiesStronger ? 14m : BasicDamage)
                .FromMonster(this)
                // .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
                .WithAttackerFx(null, AttackSfx) // 攻击音效
                .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
                .Execute(null);
            await CreatureCmd.GainBlock(Creature, MiyabiModConfig.MiyabiEnemiesStronger ? 20m : BasicBlock, ValueProp.Move, null);
        }

        private async Task PowerUp(IReadOnlyList<Creature> targets)
        {
            TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-MIYABI_GHOST_ENEMY.moves.POWER_UP.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);

            decimal slipperyAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 4m : 2m;
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, slipperyAmount * CombatState.Players.Count, base.Creature, null);

            decimal thornsAmount = MiyabiModConfig.MiyabiEnemiesStronger ? 2m : 1m;
            await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, thornsAmount, base.Creature, null);
        }
    }
}
