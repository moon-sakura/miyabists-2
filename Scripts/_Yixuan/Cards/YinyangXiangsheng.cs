using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 阴阳相生 - 1费Uncommon技能卡
    /// 下一次获得格挡时，获得对应数值的活力，或者下一次造成伤害时，获得对应数值的荆棘
    /// 升级后添加保留
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class YinyangXiangsheng : YixuanCardBase
    {
        public YinyangXiangsheng() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        //protected override string ArtPath => "res://images/_YiXuan/cards/yinyangXiangsheng.png";

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<YinyangXiangshengPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
