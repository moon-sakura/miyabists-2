using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    //[Pool(typeof(StatusCardPool))]
    internal class TrainAttack : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/trainAtk.png";
        public TrainAttack() : base(0, CardType.Status, CardRarity.Token, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(3)
        ];
    }
}
