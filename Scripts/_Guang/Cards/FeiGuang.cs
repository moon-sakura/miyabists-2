using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 飞光 - 罕见技能卡
    /// 消耗所有手卡，每消耗1张获得1点青溟剑势。
    /// 附加：使用后青溟剑势大于6点，则将消耗的卡各复制一份加入手卡，然后进入明心镜状态。
    /// 升级：0费。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class FeiGuang : GuangCardBase, I2MingxinJing
    {
        public FeiGuang() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/feiGuang.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiPerCard", 1),
            new DynamicVar("MingxinThreshold", 6),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardPile handPile = PileType.Hand.GetPile(Owner);
            int count = handPile.Cards.Count();
            var cardsToExhaust = handPile.Cards.ToList();

            // 消耗所有手卡（本卡自身不在手卡中，无需排除）
            foreach (CardModel card in cardsToExhaust)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }

            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                count * DynamicVars["JianshiPerCard"].IntValue, Owner.Creature, this);

            // 附加：青溟剑势大于6点时，把消耗的卡各复制一份加入手卡，然后进入明心镜状态
            int jianshi = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
            if (jianshi >= DynamicVars["MingxinThreshold"].IntValue
                && !Owner.Creature.HasPower<MingxinJingPower>())
            {
                // 先加回手卡：进入明心镜状态时这些复制品会和其它原卡一起变成明心镜卡
                foreach (CardModel card in cardsToExhaust)
                {
                    CardModel copy = card.CreateClone();
                    if (copy != null)
                    {
                        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
                    }
                }

                await PowerCmd.Apply<MingxinJingPower>(choiceContext, Owner.Creature,
                    1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后0费
        }
    }
}
