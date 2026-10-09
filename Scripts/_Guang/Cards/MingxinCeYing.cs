using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
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
                .Where(c => c.GetType() != typeof(MingxinCeYing) && !c.CanonicalKeywords.Contains(CardKeyword.Unplayable) && !c.CanonicalKeywords.Contains(MiyabiKeywords.EndSkill))
                .GroupBy(c => c.GetType())
                .Select(g => g.First())
                .ToList();

            foreach (CardModel card in lightCards)
            {
                // 需要指定敌人的卡牌：随机挑一个可攻击的敌人；没有敌人则跳过
                //Creature target = null;
                //if (card.TargetType == TargetType.AnyEnemy || card.TargetType == TargetType.RandomEnemy)
                //{
                //    target = Owner.Creature.CombatState.HittableEnemies
                //        .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
                //    if (target == null) continue;
                //}

                await CardCmd.AutoPlay(choiceContext, card, null);

                await CardCmd.Exhaust(choiceContext, card, false, true);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后1费
        }
    }
}
