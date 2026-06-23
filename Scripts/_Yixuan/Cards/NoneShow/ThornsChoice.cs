using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards.NoneShow
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class ThornsChoice : YixuanCardBase
    {
        public ThornsChoice() : base(-1, CardType.Status, CardRarity.Token, TargetType.None, false)
        {
        }
    }
}
