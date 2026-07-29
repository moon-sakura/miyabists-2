using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 万军诛绝 - Token技能卡（煊赫车辇选项）
    /// 获得2点能量，抽2张牌
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class WanjunZhujue : MiyabiCardBase
    {
        public WanjunZhujue() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/wanjunZhujue.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(2),
            new CardsVar(2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust,
        ];
    }
}
