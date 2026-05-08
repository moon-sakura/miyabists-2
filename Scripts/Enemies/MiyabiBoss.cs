using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Enemies
{
    internal class MiyabiBoss : CustomMonsterModel
    {
        // 根据进阶提高最小血量
        public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 根据进阶提高最大血量
        public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 轻击伤害
        private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 5);
        private int BasicBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 8);

        // 重击伤害
        private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 42, 30);

        // 多段攻击的数值
        private int MultiHitDamage => 6;
        private int MultiHitCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 4);

        // 怪物场景，如果你的场景没有挂载脚本，参考这个
        //public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://test/scenes/test_monster.tscn");

        // 如果你挂载了自己的自定义脚本，使用这个
        public override string? CustomVisualPath => "res://scenes/miyabi_char.tscn";


        // 战斗开始时，在这里给自己上buff之类
        public override async Task AfterAddedToRoom()
        {

        }

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            List<MonsterState> list = new List<MonsterState>();

            // 意图1：造成伤害，获得格挡
            var basicAttack = new MoveState(
                "BASIC_MOVE", // 状态ID
                BasicMove, // 执行函数，或者直接用lambda也可
                                 // 以下是可变参数，可以填写任意数量的意图，全部展示
                new SingleAttackIntent(BasicDamage),
                new DefendIntent()
            );

            // 意图2：重击
            var heavyAttack = new MoveState(
                "EJIZHAN",
                EJiZhan,
                new SingleAttackIntent(HeavyDamage)
            );

            var multiHitAttack = new MoveState(
                "MINGCANXUE",
                MingCanXue,
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
        private async Task BasicMove(IReadOnlyList<Creature> targets)
        {
            // 说话
            //TalkCmd.Play(L10NMonsterLookup("TEST-TEST_MONSTER.moves.BASIC_ATTACK.banter"), Creature, VfxColor.Blue);
            await DamageCmd
                .Attack(BasicDamage)
                .FromMonster(this)
                // .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
                .WithAttackerFx(null, AttackSfx) // 攻击音效
                .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
                .Execute(null);
            await CreatureCmd.GainBlock(Creature, BasicBlock, ValueProp.Move, null);
        }

        private async Task PowerUp(IReadOnlyList<Creature> targets)
        {
            TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-MIYABI_BOSS.moves.POWER_UP.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
            await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, 3m, base.Creature, null);
        }

        private async Task EJiZhan(IReadOnlyList<Creature> targets) 
        {
            await DamageCmd
                    .Attack(HeavyDamage)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);
        }

        private async Task MingCanXue(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(MultiHitDamage)
                    .WithHitCount(MultiHitCount)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);
        }

    }
}
