using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
using MegaCrit.Sts2.Core.Models;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class YoushiHuoliXueshuo : MiyabiPartnerCardBase
    {
        public YoushiHuoliXueshuo() : base(3, CardRarity.Uncommon, TargetType.None, CardType.Skill) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("SummonCount", 2),
            new DamageVar(6, ValueProp.Unpowered),
            new DynamicVar(DazeVarName, 6),
            new DynamicVar("Shield", 6),
            new DynamicVar(SupportVarName, 2),
        ];

        protected override string ArtPath => "res://images/cards/youshiHuolixueshuo.png";

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 1. First, summon Bangboos
            int summonCount = DynamicVars["SummonCount"].IntValue;
            for (int i = 0; i < summonCount; i++)
            {
                await MiyabiCombatService.SummonBangbooRandom(choiceContext, Owner);
            }

            // 2. Count Bangboos on the field after summoning
            int bangbooCount = Owner.Creature.Pets.Count(pet => pet.IsMonster && pet.Monster is MiyabiBangbooBase);

            // 3. For each Bangboo, deal damage and stagger to ALL enemies
            if (bangbooCount > 0)
            {
                int totalDamage = DynamicVars.Damage.IntValue * bangbooCount;

                // Apply stagger to each enemy
                //if (DynamicVars.TryGetValue(DazeVarName, out DynamicVar daze))
                //{
                //    foreach (Creature enemy in Owner.Creature.CombatState.Enemies.Where(c => c.IsAlive))
                //    {
                //        var totalDazeVar = new DynamicVar(DazeVarName, daze.IntValue * bangbooCount);
                //        await MiyabiCombatService.AddDaze(choiceContext, enemy, totalDazeVar, Owner.Creature);
                //    }
                //}

                // Deal AOE damage
                await DamageCmd.Attack(totalDamage)
                    .FromCard(this)
                    .TargetingAllOpponents(base.CombatState)
                    .Execute(choiceContext);
            }

            // 4. Support points: gain block per Bangboo
            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, bangbooCount));
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            int bangbooCount = Owner.Creature.Pets.Count(pet => pet.IsMonster && pet.Monster is MiyabiBangbooBase);

            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
            {
                var totalDazeVar = new DynamicVar("Daze", dazeVar.IntValue * bangbooCount);
                await MiyabiCombatService.AddDaze(choiceContext, target, totalDazeVar, base.Owner.Creature);
            }
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, int bangbooCount)
        {
            if (DynamicVars.TryGetValue("Shield", out DynamicVar shield))
            {
                var totalBlockVar = new BlockVar(shield.IntValue * bangbooCount, ValueProp.Unpowered);
                await CreatureCmd.GainBlock(Owner.Creature, totalBlockVar, null);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["SummonCount"].UpgradeValueBy(1);
            base.OnUpgrade();
        }
    }
}
