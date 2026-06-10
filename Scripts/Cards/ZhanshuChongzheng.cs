using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class ZhanshuChongzheng : MiyabiBlockCardBase
    {
        protected override string ArtPath => $"res://images/cards/zhanshuChongzheng.png";

        public ZhanshuChongzheng() : base(0, CardRarity.Common, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(2)
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
        ];

        public override bool GainsBlock => false;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IEnumerable<CardModel> cardModel = (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner));
            if (cardModel != null)
            {
                foreach (var card in cardModel)
                {
                    if (card.CanonicalKeywords.Contains(MiyabiKeywords.Friends))
                    {
                        await PowerCmd.Apply<SupportPointPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
                    }
                }
            }
        }


        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);

            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
