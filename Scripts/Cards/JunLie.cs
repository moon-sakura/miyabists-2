using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class JunLie : MiyabiCardBase
    {
        public JunLie() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<JunLiePower>(Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            this.AddKeyword(CardKeyword.Innate);
        }
    }
}
