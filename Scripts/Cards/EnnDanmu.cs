using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class EnnDanmu : MiyabiPartnerCardBase
    {
        public EnnDanmu() : base(1, CardRarity.Uncommon, TargetType.RandomEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 8),
            new DynamicVar("Times", 1),
            new DynamicVar(SupportVarName, 1),
        ];

        protected override string ArtPath => "res://images/cards/ennDanmu.png";
        

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // Count Bangboos on the field
            int bangbooCount = Owner.Creature.Pets.Count(pet => pet.IsMonster && pet.Monster is MiyabiBangbooBase);
            int totalAttacks = DynamicVars["Times"].IntValue + bangbooCount;

            IEnumerable<Creature> enemies = Owner.Creature.CombatState.Enemies.Where(c => c.IsAlive);

            // Attack first (before summoning)
            for (int i = 0; i < totalAttacks; i++)
            {
                Creature target = enemies.TakeRandom(1, Owner.RunState.Rng.Shuffle).FirstOrDefault();
                if (target == null) break;

                // Apply stagger (失衡)
                if (DynamicVars.TryGetValue(DazeVarName, out DynamicVar daze))
                    await MiyabiCombatService.AddDaze(choiceContext, target, daze, Owner.Creature);

                // Deal damage
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .Execute(choiceContext);

                if (target.IsDead)
                {
                    enemies = Owner.Creature.CombatState.Enemies.Where(c => c.IsAlive);
                }
            }

            // Support point: summon a random Bangboo (after attacking)
            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await SummonBangboo(choiceContext));
        }

        async Task SummonBangboo(PlayerChoiceContext choiceContext)
        {
            await MiyabiCombatService.SummonBangbooRandom(choiceContext, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Times"].UpgradeValueBy(1);
            base.OnUpgrade();
        }
    }
}
