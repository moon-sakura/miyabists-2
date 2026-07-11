using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class HollowJuwukongju : MiyabiCardBase
    {
        public HollowJuwukongju()
            : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        protected override string ArtPath => "res://images/cards/hollow.png";
        public override int MaxUpgradeLevel => 0;
    }
}
