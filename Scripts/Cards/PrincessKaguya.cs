using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class PrincessKaguya : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/princessKaguya.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends
        ];

        public override int MaxUpgradeLevel => 100;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Kaguya", 1),
        ];

        public PrincessKaguya()
            : base(0, CardType.Power, CardRarity.Token, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (DynamicVars.TryGetValue("Kaguya", out DynamicVar var))
                await PowerCmd.Apply<KaguyaPower>(base.Owner.Creature, var.BaseValue, base.Owner.Creature, this);
            foreach (CardModel Card in base.Owner.PlayerCombatState.AllCards)
            {
                if (Card is PrincessInoha)
                {
                    await CardCmd.Exhaust(choiceContext, Card);
                }
            }
        }

        protected override void OnUpgrade()
        {
            base.OnUpgrade();
            if(DynamicVars.TryGetValue("Kaguya", out DynamicVar var))
            {
                var.UpgradeValueBy(1);
            }
        }
    }
}
