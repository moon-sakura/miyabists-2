using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 照破无明 - 稀有能力卡
    /// 获得1层无明，将一张自身的复制加入抽牌堆，将一张遗忘加入弃牌堆，抽1张卡。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhaoPoWuMing : GuangCardBase
    {
        public ZhaoPoWuMing() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhaoPoWuMing.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<WuMingPower>(),
            HoverTipFactory.FromCard<YiWang>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 获得1层无明
            await PowerCmd.Apply<WuMingPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // 将一张自身的复制加入抽牌堆
            CardModel copy = this.CreateClone();
            if (copy != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, Owner, CardPilePosition.Random);
            }

            // 将一张遗忘加入弃牌堆
            //CardModel forget = Owner.Creature.CombatState?.CreateCard<YiWang>(Owner);
            //if (forget != null)
            //{
            await CardPileCmd.AddToCombatAndPreview<YiWang>(Owner.Creature, PileType.Discard, 1, Owner);
            //}

            // 抽1张卡
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue ,Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }
    }
}
