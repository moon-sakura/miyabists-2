using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class PrincessInoha : MiyabiCardBase
    {
        //protected override string ArtPath => "res://images/cards/SPxiaoye.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
            CardKeyword.Unplayable
        ];

        public PrincessInoha()
            : base(-1, CardType.Curse, CardRarity.None, TargetType.None)
        {
        }

        //public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
        //{
        //    if (card.Owner == base.Owner)
        //    {
        //        if (PileType.Hand.GetPile(base.Owner).Cards.Any(c => c is PrincessYears)
        //            && PileType.Hand.GetPile(base.Owner).Cards.Any(c => c is PrincessInoha))
        //        {
        //            await CardCmd.Exhaust(null, this);
        //        }
        //    }
        //}

        bool isActived = false;

        public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
        {

            if (card.Owner != base.Owner || !(card is PrincessYears || card is PrincessInoha)) return;

            if (isActived) return;

            // 获取手牌引用
            var hand = base.Owner.PlayerCombatState.Hand.Cards.ToList();

            // 检查是否两张卡都在手牌中
            bool hasYears = hand.Any(c => c is PrincessYears);
            bool hasInoha = hand.Any(c => c is PrincessInoha);

            if (hasYears && hasInoha)
            {
                var yearsInstance = hand.FirstOrDefault(c => c is PrincessYears);
                //var inohaInstance = hand.FirstOrDefault(c => c is PrincessInoha);

                //await CardCmd.Exhaust(null, yearsInstance);
                //await CardCmd.Exhaust(null, inohaInstance);

                foreach (CardModel allCard in base.Owner.PlayerCombatState.AllCards)
                {
                    if (allCard.IsUpgradable)
                    {
                        // 只有不在消耗列表里的才升级（保险起见）
                        if (!(allCard is PrincessYears || allCard is PrincessInoha))
                        {
                            // Upgrade 内部会自动判断是否能升级，不需要额外判断
                            CardCmd.Upgrade(allCard);
                        }
                    }
                }

                CardModel card2 = base.Owner.Creature.CombatState?.CreateCard<PrincessKaguya>(base.Owner); ;
                await CardCmd.Transform(yearsInstance, card2);

            }
        }
    }
}
