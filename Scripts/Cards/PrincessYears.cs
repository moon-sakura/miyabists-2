using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class PrincessYears : MiyabiCardBase
    {
        //protected override string ArtPath => "res://images/cards/SPxiaoye.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
            CardKeyword.Unplayable,
            CardKeyword.Retain
        ];

        public PrincessYears()
            : base(-1, CardType.Curse, CardRarity.None, TargetType.None)
        {
        }

        
    }
}
