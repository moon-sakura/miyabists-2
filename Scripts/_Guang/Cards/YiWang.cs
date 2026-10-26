using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 遗忘 - 诅咒卡
    /// 在手卡中时必须优先打出：选择1张手卡消耗，然后消耗自身。
    /// 不可升级。
    /// </summary>
    [RegisterCard(typeof(CurseCardPool))]
    internal class YiWang : MiyabiCardBase
    {
        public YiWang() : base(1, CardType.Curse, CardRarity.Curse, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_Guang/cards/common.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        public override int MaxUpgradeLevel => 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 手卡为空时没有可选的卡，直接结束
            if (!PileType.Hand.GetPile(Owner).Cards.Any()) return;

            // 选择1张手卡消耗（本卡自身已在结算中，不在手卡里）
            CardModel? target = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext,
                player: Owner,
                filter: null,
                source: this)).FirstOrDefault();

            if (target != null)
            {
                await CardCmd.Exhaust(choiceContext, target);
            }
        }

        /// <summary>
        /// 只要《遗忘》还在手卡里，其它手卡就不能先打出（自动打出不受影响）。
        /// </summary>
        public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
        {
            if (card is YiWang) return true;
            if (autoPlayType != AutoPlayType.None) return true;
            if (Pile?.Type != PileType.Hand) return true;
            return card?.Pile?.Type != PileType.Hand;
        }
    }
}
