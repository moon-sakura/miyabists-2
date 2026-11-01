using MegaCrit.Sts2.Core.Combat;
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

        public bool isCopiedThisTurn = false; // 标记本回合是否已经复制过

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this || isCopiedThisTurn) return;

            isCopiedThisTurn = true;

            // 向抽卡堆中加入一张本卡的复制
            CardModel copy = this.CreateClone();
            if (copy != null)
            {
                ((WuweiQiyue)copy).isCopiedThisTurn = true; // 标记复制的卡也已经复制过，避免无限复制
                await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, Owner, CardPilePosition.Random);
            }
        }

        public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            isCopiedThisTurn = false; // 重置复制标记
            return base.BeforeSideTurnEnd(choiceContext, side, participants);
        }

        // ========== 待实现 ==========
        // TODO: 这张卡在手卡时造成的伤害变为减少等量最大生命值
        // 该效果需要在手牌中全局改写伤害结算（伤害 → 等量最大生命值减少），
        // 暂留空待实现。
        public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target == null || cardSource == null) return amount;

            if (Pile.Type != PileType.Hand) return amount; // 仅在手牌中生效

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
