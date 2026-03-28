using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    internal class TianmiJingxia : MiyabiPartnerCardBase
    {
        public TianmiJingxia() : base(1, CardRarity.Common, TargetType.Self, CardType.Power) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<TianmijxPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            this.AddKeyword(CardKeyword.Innate);
        }
    }
}
