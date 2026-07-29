using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    /// 无拘剑势 - Token能力卡（煊赫车辇选项）
    /// 每次使用攻击卡时，对随机敌人造成4点伤害
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class WujuJianshi : MiyabiCardBase
    {
        public WujuJianshi() : base(-1, CardType.Power, CardRarity.Token, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/wujuJianshi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DamagePerAttack", 4),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<WujuJianshiPower>(),
        ];
    }
}
