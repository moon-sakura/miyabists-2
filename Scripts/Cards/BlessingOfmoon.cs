using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    internal class BlessingOfmoon : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/moonBlessing.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.OtherWorldFriends];

        public BlessingOfmoon()
            : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Bless", 25)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue("Bless", out DynamicVar b))
                await PowerCmd.Apply<BlessingMoonPower>(base.Owner.Creature, b.BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("Bless", out DynamicVar b)) b.UpgradeValueBy(15);
        }
    }
}
