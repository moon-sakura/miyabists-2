using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    internal class ByeQuanleida : MiyabiPartnerCardBase
    {
        public ByeQuanleida():base(1,CardRarity.Rare,TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new BlockVar(0,ValueProp.Move),
            new CardsVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            base.OnPlay(choiceContext, cardPlay);

            bool isBreak = cardPlay.Target.HasPower<BreakPower>();
            if (base.CheckSupportCost(2) != 0 || isBreak)
            {
                //选择一张攻击卡加入手卡
                CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, DynamicVars.Cards.IntValue);
                List<CardModel> cardsIn = (from c in PileType.Discard.GetPile(base.Owner).Cards
                                           where c.Type == CardType.Attack
                                           orderby c.Rarity, c.Id
                                           select c).ToList();
                if (cardsIn.Count != 0)
                {
                    IEnumerable<CardModel> cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs));
                    if (cardModel != null)
                    {
                        foreach(CardModel card in cardModel)
                        {
                            card.DynamicVars.Damage.BaseValue += 3;
                            await CardPileCmd.Add(cardModel, PileType.Hand); 
                        }
                    }
                }

                if(!isBreak)
                    await CostSupporPoint(2);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(6);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
            DynamicVars.Block.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p)) p.UpgradeValueBy(1);

        }
    }
}
