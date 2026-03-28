using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class QuickParry : MiyabiBlockCardBase
    {
        //public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public QuickParry() : base(1, CardRarity.Common, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(2, ValueProp.Move),
            new DynamicVar(ParryVarName, 1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 1. 获取抽牌堆
            CardPile discardPile = PileType.Draw.GetPile(base.Owner);

            // 2. 筛选出含有 Friends 关键字的卡，并随机抽取
            // 注意：StS2 的 Cards 属性通常是 IEnumerable<CardModel>，我们可以直接用 LINQ 筛选
            IEnumerable<CardModel> selectedCards = discardPile.Cards
                .Where(c => c.CanonicalKeywords.Contains(MiyabiKeywords.Friends)) // 筛选符合条件的卡
                .TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection); // 随机取 N 张

            if (selectedCards.Count() != 0)
            {
                foreach (CardModel item in selectedCards)
                {
                    await CardPileCmd.Add(item, PileType.Hand);
                }
            }

            await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature, 1, null, null);
        }


        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
