using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    //[RegisterCard(typeof(MiyabiCardPool))]
    internal class Kakania : MiyabiPartnerCardBase
    {
        public Kakania() : base(1, CardRarity.Rare, TargetType.Self, CardType.Power)
        {
        }

        public override string PortraitPath => $"res://images/cards/kakania.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("BurstRatio", 30),
            new DynamicVar(SupportVarName, 5),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var power = await PowerCmd.Apply<KakaniaPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            power.SetBurstRatio(DynamicVars["BurstRatio"].IntValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BurstRatio"].UpgradeValueBy(20);
        }
    }
}
