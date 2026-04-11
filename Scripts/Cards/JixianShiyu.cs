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
    internal class JixianShiyu : MiyabiCardBase
    {
        public JixianShiyu() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(1),
            new DynamicVar("SLIPPERY_POWER", 2)
        ];

        public override bool GainsBlock => false;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SlipperyPower>(),
            HoverTipFactory.FromCard<HanQue>()
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (DynamicVars.TryGetValue("SLIPPERY_POWER", out DynamicVar s))
                await PowerCmd.Apply<SlipperyPower>(base.Owner.Creature, s.BaseValue, Owner.Creature, this);

            for(int i = 0; i < DynamicVars.Cards.BaseValue; i++)
            {
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<HanQue>(base.Owner.Creature.Player);
                if (base.IsUpgraded) reward1.UpgradeInternal();
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
            }

            await PowerCmd.Apply<NoDrawPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);

        }


        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
            EnergyCost.UpgradeBy(-1);
        }
    }
}
