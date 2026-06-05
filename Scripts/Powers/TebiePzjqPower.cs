
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TebiePzjqPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/teshuPzjq.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [

        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DazeVuln", 0),
        ];

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            foreach (Creature enemy in Owner.CombatState.Enemies.ToList())
            {
                int result = base.Owner.Player.RunState.Rng.Shuffle.NextInt(1, 11);
                if (enemy.Monster.IntendsToAttack)
                {
                    var duelAttack = new MoveState(
                        "MIYABI_TBPZJQ_DONOTHING",
                        async targets => await DamageCmd
                        .Attack(0)
                        .Unpowered()
                        //.Targeting(base.Owner.Creature)
                        .FromMonster(enemy.Monster)
                        //.WithAttackerFx()
                        //.WithHitFx("vfx/vfx_attack_blunt")
                        .Execute(null),
                    new SingleAttackIntent(0))
                    {
                        FollowUpStateId = enemy.Monster.NextMove.StateId,
                        MustPerformOnceBeforeTransitioning = true
                    };

                    if (result <= Amount)
                    {
                        enemy.Monster.SetMoveImmediate(duelAttack, true);
                    }
                }
            }
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power is BreakPower && amount > 0)
            {
                await PowerCmd.Apply<DazeVulnPower>(choiceContext, power.Owner, DynamicVars["DazeVuln"].BaseValue, null, null);
            }
        }

        public void SetDazeVuln(int amount)
        {
            base.DynamicVars["DazeVuln"].BaseValue = amount;
        }
    }
}
