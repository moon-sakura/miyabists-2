using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class RanyouyinShare : MiyabiBlockCardBase
    {
        public RanyouyinShare() : base(0, CardRarity.Common, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Decible", 5)
        ];

        public override bool GainsBlock => false;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<VulnerablePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            bool hasIt = base.Owner.Relics.Any(r => r is SwordNotailRelic);
            int n = 0;
            if (base.DynamicVars.TryGetValue("Decible", out var d))
                n = d.IntValue;
            if (hasIt) 
            {
                await MiyabiCombatService.AddDecible(Owner, n);
            }
        }

        public override async Task BeforeCardPlayed(CardPlay cardPlay)
        {
            int amount = CombatManager.Instance.History.CardPlaysFinished
                .Count((CardPlayFinishedEntry e)
                => e.CardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                && e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState));
            if (amount > 0)
            {
                BaseReplayCount = 1;
            }
        }


        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("Decible", out var d)) d.UpgradeValueBy(2);
        }
    }
}
