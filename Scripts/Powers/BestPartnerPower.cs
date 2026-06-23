using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class BestPartnerPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/bamianWeifeng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private int count = 0;

        private int max = 6;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player) return;

            count++;

            if(count % 2 == 0 && count <= max)
            {
                await CreatureCmd.Heal(Owner, Amount);
            }
        }

        public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if(side == Owner.Side)
            {
                count = 0;
            }

            return base.AfterSideTurnEnd(choiceContext, side, participants);
        }
    }
}
