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
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
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
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后0费
        }
    }
}
