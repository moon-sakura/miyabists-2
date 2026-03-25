using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    internal class HuaCi : MiyabiAttackCardBase
    {
        //public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public HuaCi() : base(0, CardRarity.Token, TargetType.AnyEnemy, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.LieShuang,
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 5)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
        }
    }
}
