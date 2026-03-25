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
    internal class ZhanshuChongzheng : MiyabiBlockCardBase
    {
        //public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public ZhanshuChongzheng() : base(0, CardRarity.Common, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(2)
        ];

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
                        await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
                    }
                }
            }
        }


        protected override void OnUpgrade()
        {
            // 升级增加 2 点护甲
            DynamicVars.Cards.UpgradeValueBy(1);

            // 如果需要升级 Parry 或 Slippery，可以在此添加逻辑
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
