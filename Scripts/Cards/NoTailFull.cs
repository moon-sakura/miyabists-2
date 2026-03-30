using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    internal class NoTailFull : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/notailFull.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            CardKeyword.Exhaust,
            CardKeyword.Ethereal
        ];

        public NoTailFull() : base(0, CardType.Power, CardRarity.Ancient, TargetType.Self){ }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if(!base.IsUpgraded)
                await PowerCmd.Apply<NoTailFullPower>(base.Owner.Creature, 1m, Owner.Creature, this);
            else
                await PowerCmd.Apply<MiyabiFullPower>(base.Owner.Creature, 1m, Owner.Creature, this);
        }

        public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if (card == this)
            {
                await CardCmd.Exhaust(choiceContext,this);
            }
        }

        protected override void OnUpgrade()
        {
            base.OnUpgrade();
        }
    }
}
