using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(MiyabiCardPool))]
    internal class FengHua : MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/fengHua.png";

        public FengHua() : base(1, CardRarity.Basic, TargetType.AnyEnemy, true) { }

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };


        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new DynamicVar(DazeVarName, 2)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
