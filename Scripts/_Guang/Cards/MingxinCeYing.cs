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

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心境·策应（原卡：策应）
    /// 将消耗堆中每种卡牌（《明心境·策应》除外）各打出一张。附加：消耗。
    /// 升级：1费。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinCeYing : GuangMingxinjingCardBase
    {
        public MingxinCeYing() : base(2, CardType.Attack, CardRarity.Token, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 消耗堆中每种卡牌各取一张（排除《明心境·策应》自身）
            CardPile exhaustPile = PileType.Exhaust.GetPile(Owner);
            List<CardModel> lightCards = exhaustPile.Cards
                .Where(c => c.GetType() != typeof(MingxinCeYing))
                .GroupBy(c => c.GetType())
                .Select(g => g.First())
                .ToList();

            foreach (CardModel card in lightCards)
            {
                // ========== 待实现 ==========
                // TODO: 打出这张卡
                // 需要"从消耗堆直接打出卡牌"的指令（本项目暂未确认对应 API），待补充。
            }

            // TODO: 若"打出"需要逐个等待结算，记得 await
            await Task.CompletedTask;
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后1费
        }
    }
}
