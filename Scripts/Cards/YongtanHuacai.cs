using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class YongtanHuacai : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/yongtanHuacai.png";
        public YongtanHuacai() : base(3, CardRarity.Rare, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(1),
            new DynamicVar(SupportVarName,2),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext));

            await PowerCmd.Apply<YongtanHuacaiPower>(choiceContext, Owner.Creature, 1, base.Owner.Creature, this);

        }

        async Task FriendFunc(PlayerChoiceContext choiceContext)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
            List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
                                       where c.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                                       orderby c.Rarity, c.Id
                                       select c).ToList();
            if (cardsIn.Count != 0)
            {
                CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs)).FirstOrDefault();
                if (cardModel != null)
                {
                    await CardPileCmd.Add(cardModel, PileType.Hand);
                }
            }
        }


        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
