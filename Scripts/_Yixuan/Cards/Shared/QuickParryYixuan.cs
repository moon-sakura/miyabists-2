using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QuickParryYixuan : QuickParry
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

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

            await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, 2, null, null);
            await PowerCmd.Apply<SupportPointPower>(choiceContext, base.Owner.Creature, 1, null, null);
        }
    }
}
