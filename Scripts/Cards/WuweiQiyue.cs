using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
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
    /// <summary>
    /// 无尾契约 - Rare能力卡（不可打出）
    /// 抽到这张卡时，向抽卡堆中加入一张这张卡的复制
    /// 这张卡在手卡时造成的伤害变为减少等量最大生命值
    /// 升级后添加虚无
    /// </summary>
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class WuweiQiyue : MiyabiCardBase
    {
        public WuweiQiyue() : base(-1, CardType.Power, CardRarity.Rare, TargetType.None)
        {
        }

        protected override string ArtPath => "res://images/cards/wuweiQiyue.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Unplayable,
        ];

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this) return;

            // 向抽卡堆中加入一张本卡的复制
            CardModel copy = this.CreateClone();
            if (copy != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, Owner, CardPilePosition.Random);
            }
        }

        // ========== 待实现 ==========
        // TODO: 这张卡在手卡时造成的伤害变为减少等量最大生命值
        // 该效果需要在手牌中全局改写伤害结算（伤害 → 等量最大生命值减少），
        // 暂留空待实现。
        public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target.IsAlive && cardSource.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang) && cardSource.Owner == base.Owner)
            {
                return 0m;
            }

            return amount;
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Ethereal); // 升级后添加虚无
        }
    }
}
