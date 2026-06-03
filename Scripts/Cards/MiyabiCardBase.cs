using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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

        protected const string DazeVarName = "DAZE_POWER";
        protected const string ParryVarName = "PARRY_POWER";
        protected const string SlipperyVarName = "SLIPPERY_POWER";
        protected const string AnomalyBuildupVarName = "ANOBUILD_POWER";
        protected const string ExhaustCountVarName = "ExhaustCount";

        //public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.LieShuang];

        protected MiyabiCardBase(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        /// <summary> 根据难度获取"X次后消耗"的基础次数，子类可重写 </summary>
        protected virtual decimal GetExhaustUses()
            => 2m;

        /// <summary> 递减消耗计数，归零时自动耗尽并重置。调用方只需一行 await </summary>
        protected async Task TryExhaustAfterUse(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card != this) return;
            if (!DynamicVars.TryGetValue(ExhaustCountVarName, out DynamicVar exhaustVar)) return;

            exhaustVar.BaseValue -= 1;
            if (exhaustVar.BaseValue <= 0)
            {
                await CardCmd.Exhaust(context, this);
                exhaustVar.BaseValue = GetExhaustUses();
            }
        }
    }
}
