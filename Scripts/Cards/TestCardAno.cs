using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TestCardAno : MiyabiPartnerCardBase
    {
        public TestCardAno()
            : base(0,  CardRarity.None, TargetType.AnyEnemy, CardType.Attack, false)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(AnomalyBuildupVarName,3)
        ];
    }
}
