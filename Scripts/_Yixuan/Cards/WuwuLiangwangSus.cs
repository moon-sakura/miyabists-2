using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 物我两忘·续 - 0费Token技能卡
    /// 获得消耗能力层数/10的能量，抽取消耗能力层数/10的卡，消耗，保留
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class WuwuLiangwangSus : YixuanCardBase
    {
        public WuwuLiangwangSus() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_YiXuan/cards/wuwuLiangwangSus.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("ExhaustedPowerCount", 0),
            new EnergyVar(0),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
            CardKeyword.Retain,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            // TODO: Add hover tips
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int exhaustedCount = DynamicVars["ExhaustedPowerCount"].IntValue;

            // 获得能量 = 消耗层数 / 10
            int energyGain = exhaustedCount;
            if (energyGain > 0)
            {
                await PlayerCmd.GainEnergy(energyGain, Owner);
            }

            // 抽卡 = 消耗层数 / 10
            int drawCount = exhaustedCount;
            if (drawCount > 0)
            {
                await CardPileCmd.Draw(choiceContext, drawCount, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            // Token卡不升级
        }
    }
}
