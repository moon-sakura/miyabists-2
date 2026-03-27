using MegaCrit.Sts2.Core.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TrainStep : MiyabiCardBase
    {
        public TrainStep() : base(0, CardType.Status, CardRarity.None, TargetType.Self, true) { }
    }
}
