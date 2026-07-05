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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class ByeQuanleida : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/byeQuanleida.png";
        public ByeQuanleida():base(1,CardRarity.Rare,TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new CardsVar(1),
            new DynamicVar(SupportVarName,2),
            new DynamicVar("Quanleida",2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

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
                    foreach (CardModel card in cardModel)
                    {
                        await CardPileCmd.Add(cardModel, PileType.Hand);
                    }
                }
            }

            bool isBreak = cardPlay.Target.HasPower<BreakPower>();

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext), isBreak, isBreak);
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext)
        {
            
            await PowerCmd.Apply<ByeQuanleidaPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
            //DynamicVars.Cards.UpgradeValueBy(1);
            DynamicVars["Quanleida"].UpgradeValueBy(1);
        }
    }
}
