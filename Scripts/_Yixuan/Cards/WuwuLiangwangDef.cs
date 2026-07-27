using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 物我两忘·御 - 0费Token技能卡
    /// 获得消耗能力层数的格挡，并对所有敌人施加相同层数的术法值，消耗，保留
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class WuwuLiangwangDef : YixuanCardBase
    {
        public WuwuLiangwangDef() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_YiXuan/cards/wuwuLiangwangDef.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(0, ValueProp.Move),
            new DynamicVar("ExhaustedPowerCount", 0),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
            CardKeyword.Retain,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShufaZhi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            DynamicVars.Block.BaseValue = DynamicVars["ExhaustedPowerCount"].BaseValue;

            // 获得格挡
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // 对所有敌人施加术法值
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, DynamicVars["ExhaustedPowerCount"].BaseValue, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            // Token卡不升级
        }
    }
}
