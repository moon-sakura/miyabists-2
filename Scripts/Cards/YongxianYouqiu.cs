using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    /// 永陷幽囚 - Token攻击卡（煊赫车辇选项）
    /// 对所有敌人造成8点伤害，击破则3倍并清除击破状态
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class YongxianYouqiu : MiyabiCardBase
    {
        public YongxianYouqiu() : base(-1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override string ArtPath => "res://images/cards/yongxianYouqiu.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8, ValueProp.Unpowered),
            new DynamicVar("BreakMultiplier", 3),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<BreakPower>(),
        ];
    }
}
