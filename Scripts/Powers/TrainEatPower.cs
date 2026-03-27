using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TrainEatPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        int turnLimit = 3;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount = turnLimit;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
                Amount--;
            }
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            await PowerCmd.Apply<TrainEatOverPower>(oldOwner, 1, null, null);
            //return Task.CompletedTask;
        }

        public override bool ShouldPlay(CardModel card, AutoPlayType _)
        {
            if (card.Owner.Creature != base.Owner)
            {
                return true;
            }
            if (card.Type != CardType.Attack)
            {
                return true;
            }
            return false;
        }
    }
}
