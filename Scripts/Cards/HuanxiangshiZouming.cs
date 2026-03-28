using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    internal class HuanxiangshiZouming : MiyabiPartnerCardBase
    {
        public HuanxiangshiZouming() : base(2, CardRarity.Uncommon, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("SupportPoint",1)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust
        ];


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
            List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
                                       where c.Type != CardType.Power
                                       orderby c.Rarity, c.Id
                                       select c).ToList();
            if (cardsIn.Count != 0)
            {
                CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs)).FirstOrDefault();
                if (cardModel != null)
                {
                    cardModel.SetToFreeThisTurn();
                    await CardPileCmd.Add(cardModel, PileType.Hand);
                }
            }

            if (base.CheckSupportCost(2) != 0)
            {
                CardSelectorPrefs prefs2 = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
                List<CardModel> cardsIn2 = (from c in PileType.Draw.GetPile(base.Owner).Cards
                                           where c.Type != CardType.Power
                                           orderby c.Rarity, c.Id
                                           select c).ToList();
                if (cardsIn2.Count != 0)
                {
                    CardModel cardModel2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs)).FirstOrDefault();
                    if (cardModel2 != null)
                    {
                        cardModel2.SetToFreeThisTurn();
                        await CardPileCmd.Add(cardModel2, PileType.Hand);
                    }
                }
                await CostSupporPoint(2);
            }

            if (base.DynamicVars.TryGetValue("SupportPoint", out DynamicVar s))
                await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature, s.BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("SupportPoint", out DynamicVar s)) s.UpgradeValueBy(2);
        }
    }
}
