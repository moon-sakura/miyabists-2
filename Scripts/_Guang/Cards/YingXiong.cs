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
    /// 英雄！ - 稀有能力卡
    /// 恢复所有能量，抽 3 张卡，进入明心镜状态。
    /// 本回合每使用 1 张卡，随机将 1 张《遗忘》或《失感》加入弃牌堆。
    /// 虚无。
    /// 升级：去掉虚无。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class YingXiong : GuangCardBase
    {
        public YingXiong() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MingxinJingPower>(),
            HoverTipFactory.FromCard<ShiGan>(),
            HoverTipFactory.FromCard<YiWang>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 恢复所有能量
            await PlayerCmd.SetEnergy(Owner.PlayerCombatState.MaxEnergy, Owner);

            // 抽 3 张卡
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);

            // 进入明心镜状态
            await PowerCmd.Apply<MingxinJingPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // 进状态会消耗所有正面能力，所以“本回合出牌加诅咒”的能力要最后挂
            await PowerCmd.Apply<YingXiongPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Ethereal); // 升级后去掉虚无
        }
    }
}
