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
    internal class TrainStep : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/trainStep.png";
        public TrainStep() : base(0, CardType.Status, CardRarity.Token, TargetType.Self) { }
    }
}
