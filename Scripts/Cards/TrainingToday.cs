using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs.Metrics;
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
    internal class TrainingToday : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/trainingToday.png";
        public TrainingToday() : base(0,CardType.Skill, CardRarity.Rare,TargetType.Self, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            TrainSpeak optionSpeak = base.Owner.Creature.CombatState?.CreateCard<TrainSpeak>(base.Owner);
            TrainAttack optionAttack = base.Owner.Creature.CombatState?.CreateCard<TrainAttack>(base.Owner);
            TrainEat optionEat = base.Owner.Creature.CombatState?.CreateCard<TrainEat>(base.Owner);
            TrainStep optionStep = base.Owner.Creature.CombatState?.CreateCard<TrainStep>(base.Owner);

            if (optionSpeak != null && optionAttack != null && optionEat != null && optionStep != null)
            {
                List<CardModel> options = new List<CardModel> { optionSpeak, optionAttack, optionEat, optionStep };

                int result = base.Owner.RunState.Rng.Shuffle.NextInt(1, options.Count + 1);
                options.RemoveRange(result - 1, 1);

                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
                if (chosen is TrainSpeak)
                {
                    await PowerCmd.Apply<TrainSpeakPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                else if (chosen is TrainAttack)
                {
                    await PowerCmd.Apply<TrainAttackPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                else if (chosen is TrainEat)
                {
                    await PowerCmd.Apply<TrainEatPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
                else if(chosen is TrainStep)
                {
                    await PowerCmd.Apply<TrainStepPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                }
            }
        }

        protected override void OnUpgrade()
        {
            base.AddKeyword(CardKeyword.Innate);
        }
    }
}
