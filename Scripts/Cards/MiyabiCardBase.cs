using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.GameInfo.Objects;
using Miyabists2.Scripts.Char;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(MiyabiCardPool))]
    public abstract class MiyabiCardBase : CustomCardModel
    {
        protected virtual string ArtPath => "res://images/cards/commonCards.png";
        //public override string PortraitPath => $"res://images/cards/{Id.Entry.ToLowerInvariant()}.png";
        public override string PortraitPath => ArtPath;
        public override string BetaPortraitPath => ArtPath;

        //public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.LieShuang];

        protected MiyabiCardBase(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }



    }
}
