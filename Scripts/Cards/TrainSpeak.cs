using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(StatusCardPool))]
    internal class TrainSpeak:MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/trainSpeak.png";
        public TrainSpeak() : base(0, CardType.Status, CardRarity.None, TargetType.Self, true) { }
    }
}
