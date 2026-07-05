using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Extensions;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class SheWen : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/sheWen.png";

        public SheWen() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("Eat", 0),
            new DynamicVar(SupportVarName,1),
            new CardsVar(2),
        ];

        bool try2Eat = true;
        int count = 0;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            try2Eat = true;

            await base.OnPlay(choiceContext, cardPlay);

            if (DynamicVars["Eat"].BaseValue < 5)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);

                CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue);
                List<CardModel> cardsIn = [];

                cardsIn.AddRange(base.Owner.PlayerCombatState.DrawPile.Cards.ToList());
                cardsIn.AddRange(base.Owner.PlayerCombatState.Hand.Cards.ToList());


                if (cardsIn.Count != 0)
                {
                    IEnumerable<CardModel> cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs));
                    if (cardModel.Count() > 0)
                    {
                        foreach (CardModel c in cardModel) 
                        {
                            await CardCmd.Exhaust(choiceContext, c);
                            DynamicVars["Eat"].BaseValue++;
                        }
                    }
                }
                else
                {
                    count++;
                }

                if (base.CheckSupportCost(DynamicVars[SupportVarName].IntValue) != 0)
                {
                    try2Eat = false;
                    await CostSupporPoint(DynamicVars[SupportVarName].IntValue, choiceContext);
                }
            }
            else
            {
                DynamicVars["Eat"].BaseValue = 0;
                await DamageCmd.Attack(15m)
                .FromCard(this, cardPlay).TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);

                foreach(Creature enemy in base.CombatState.Enemies)
                {
                    await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 2, base.Owner.Creature, this);
                }

                try2Eat = false;
            }

            if (try2Eat || count >= 3)
            {
                int result = base.Owner.RunState.Rng.Shuffle.NextInt(1, 11);

                if(result == 1 || count >= 3)
                {
                    IEnumerable<CardModel> card = base.Owner.PlayerCombatState.AllCards
                        .Where(c => c is not SheWen)
                        .TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection);

                    foreach (CardModel c in card)
                    { 
                        await CardCmd.Exhaust(choiceContext, c);
                        DynamicVars["Eat"].BaseValue++;
                    }

                    count = 0;
                }
            }
        }

        public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if (card != this) return;
            count++;
            if (count >= 3)
            {
                IEnumerable<CardModel> cards = base.Owner.PlayerCombatState.AllCards
                        .Where(c => !(c is SheWen))
                        .TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection);

                foreach (CardModel c in cards)
                {
                    await CardCmd.Exhaust(choiceContext, c);
                    DynamicVars["Eat"].BaseValue++;
                }
                count = 0;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
