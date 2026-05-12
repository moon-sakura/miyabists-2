using BaseLib.Abstracts;
using BaseLib.Extensions;
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
    internal class MiyabiBoss : CustomMonsterModel
    {
        // 根据进阶提高最小血量
        public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 根据进阶提高最大血量
        public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 170, 170);

        // 轻击伤害
        private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);
        private int BasicBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 15);

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
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 10m * CombatState.Players.Count, base.Creature, null);
        }

        private bool HeavyAttackUsed = false;
        private bool MultiHitUsed = false;

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
                new BuffIntent(),
                new DefendIntent()
            );

            var dashAttack = new MoveState(
                "DASH_ATTACK",
                DashAttackF,
                new SingleAttackIntent(BasicDamage),
                new BuffIntent()
            );



            RandomBranchState randomBranchState = new RandomBranchState("ATK");
            randomBranchState.AddBranch(multiHitAttack, MoveRepeatType.CannotRepeat);
            randomBranchState.AddBranch(heavyAttack, MoveRepeatType.CannotRepeat);

            RandomBranchState randomBranchState2 = new RandomBranchState("SLIP");
            randomBranchState2.AddBranch(powerUp, MoveRepeatType.CannotRepeat);
            randomBranchState2.AddBranch(dashAttack, MoveRepeatType.CannotRepeat);

            ConditionalBranchState conditionalBranchState = new ConditionalBranchState("COND");

            conditionalBranchState.AddState(heavyAttack,
                () =>
                {
                    foreach(Player player in base.Creature.CombatState.Players)
                    {
                        if ((player.Creature.HasPower<BreakPlayerPower>() && player.Creature.HasPower<SlipperyPower>())
                        || (player.PlayerCombatState.DrawPile.Cards.CountBy(c => !c.GainsBlock).Count() < 3
                        && !(player.Creature.HasPower<SlipperyPower>() || player.Creature.HasPower<IntangiblePower>()))
                        )
                        {
                            if (!HeavyAttackUsed)
                            {
                                HeavyAttackUsed = true;
                                MultiHitUsed = false;
                                return true;
                            }
                        }
                    }
                    return false;
                });

            conditionalBranchState.AddState(multiHitAttack,
                () =>
                {
                    foreach (Player player in Creature.CombatState.Players)
                    {
                        if (player.Creature.HasPower<BreakPlayerPower>()
                        || (player.PlayerCombatState.DrawPile.Cards.CountBy(c => !c.GainsBlock).Count() < 3
                        && !(player.Creature.HasPower<IntangiblePower>()))
                        )
                        {
                            if (!MultiHitUsed)
                            {
                                MultiHitUsed = true;
                                HeavyAttackUsed = false;
                                return true;
                            }
                        }
                    }
                    return false;
                });

            conditionalBranchState.AddState(randomBranchState2,
                () =>
                {
                    foreach (Player player in Creature.CombatState.Players)
                    {
                        if (!player.Creature.CombatState.Enemies.FirstOrDefault().HasPower<SlipperyPower>())
                        {
                            MultiHitUsed = false;
                            HeavyAttackUsed = false;
                            return true;
                        }
                    }
                    return false;
                });

            conditionalBranchState.AddState(powerUp, () => 
            {
                MultiHitUsed = false;
                HeavyAttackUsed = false;
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

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (dealer == base.Creature && target.IsPlayer && target.IsAlive)
            {
                await MiyabiCombatService.DazeAddtoPlayer(choiceContext, target, result.UnblockedDamage * 5);

                ///
                ///
                ///

                MiyabiCombatService.SetFrostTriggerMultiply(base.Creature);
                int trigger = 50;

                int chkFB = target.GetPowerAmount<FrostBuildPower>() + result.TotalDamage;

                // 确保是本卡造成的实际伤害，且目标存活
                if (result.TotalDamage > 0 && chkFB <= trigger && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
                {
                    await PowerCmd.Apply<FrostBuildPower>(choiceContext, target, result.TotalDamage, base.Creature, null);
                }
                //烈霜积蓄值积攒逻辑
                if (chkFB >= trigger + 1)
                {
                    //await MiyabiCombatService.FrostApply(target,base.Owner.Creature,choiceContext);
                    //await PowerCmd.SetAmount<FrostBuildPower>(target, 1, base.Owner.Creature, this);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<FrostBuildPower>(), 1, dealer, null);
                    //await PowerCmd.Apply<FrostPower>(choiceContext, target, 1, base.Creature, null);
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 10m, ValueProp.Unpowered, (Creature)null);

                    //int fireAmount = target.GetPowerAmount<FrostFirePower>();
                    //await CreatureCmd.Damage(null, target, 10m, ValueProp.Unpowered, dealer);


                    if (target.HasPower<AttributeAnomalyPower>())
                    {
                        await MiyabiCombatService.DisorderApply(target, base.Creature, choiceContext);
                    }
                    else
                    {
                        await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, base.Creature, null);
                    }
                }
            }
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
            //TalkCmd.Play(L10NMonsterLookup("MIYABISTS2-MIYABI_BOSS.moves.POWER_UP.banter"), Creature, VfxColor.Blue);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 4m * targets.Count, base.Creature, null);
            await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), base.Creature, 3m, base.Creature, null);

            foreach (var target in targets) 
            { 
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, 3m, base.Creature, null);
                await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), target, 3m, base.Creature, null);
            }

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

        private async Task DashAttackF(IReadOnlyList<Creature> targets)
        {
            await DamageCmd
                    .Attack(BasicDamage)
                    .FromMonster(this)
                    .WithAttackerFx(null, AttackSfx)
                    .WithHitFx("vfx/vfx_giant_horizontal_slash")
                    .Execute(null);

            await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), base.Creature, 4m * targets.Count, base.Creature, null);
        }
    }
}
