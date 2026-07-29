using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 凯旋坦途 - Token技能卡（煊赫车辇选项）
    /// 获得2点力量，2点敏捷
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class KaixuanTantu : MiyabiCardBase
    {
        public KaixuanTantu() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/kaixuanTantu.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("StrengthAmount", 2),
            new DynamicVar("DexterityAmount", 2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<StrengthPower>(),
            //HoverTipFactory.FromPower<DexterityPower>(),
        ];
    }
}
