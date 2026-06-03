using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(StatusCardPool))]
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
                await PowerCmd.Apply<KaguyaPower>(choiceContext, base.Owner.Creature, var.BaseValue, base.Owner.Creature, this);
            var inohas = base.Owner.PlayerCombatState.AllCards.Where(c => c is PrincessInoha).ToList();
            foreach (CardModel Card in inohas)
            {
                await CardCmd.Exhaust(choiceContext, Card);
                await DoRandomEffectSP(choiceContext);
            }
        }

        private async Task DoRandomEffectSP(PlayerChoiceContext choiceContext)
        {
            int effect =  base.Owner.RunState.Rng.Shuffle.NextInt(1, 7);
            switch (effect)
            {
                case 1:
                    await CreatureCmd.Heal(base.Owner.Creature, Owner.Creature.MaxHp * 0.2m);
                    break;
                case 2:
                    foreach (Creature Enemy in base.Owner.Creature.CombatState.HittableEnemies)
                    {
                        await CreatureCmd.Damage(choiceContext, Enemy, Enemy.MaxHp * 0.1m, ValueProp.Unpowered, null, null);
                    }
                    break;
                case 3:
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 3m, base.Owner.Creature, null);
                    break;
                case 4:
                    await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, 8m, base.Owner.Creature, null);
                    break;
                case 5:
                    foreach (Creature Enemy in base.Owner.Creature.CombatState.Enemies)
                    {
                        await PowerCmd.Apply<WeakPower>(choiceContext, Enemy, 3m, base.Owner.Creature, null);
                    }
                    break;
                case 6:
                    foreach (Creature Enemy in base.Owner.Creature.CombatState.Enemies)
                    {
                        await PowerCmd.Apply<VulnerablePower>(choiceContext, Enemy, 3m, base.Owner.Creature, null);
                    }
                    break;
                default:
                    break;
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
