using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class XiezuoShigong : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/xiezuoShigong.png";

        public XiezuoShigong() : base(1, CardRarity.Common, TargetType.RandomEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(2, ValueProp.Move),
            new DynamicVar(DazeVarName, 1),
            new DynamicVar(AnomalyBuildupVarName,1),
            new DynamicVar("RepeatCount", 1)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AnomalyBuildupPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            IEnumerable<Creature> enemies = base.Owner.Creature.CombatState.Enemies.Where(c => c.IsAlive);
            for (int i = 0; i < DynamicVars["RepeatCount"].BaseValue; i++)
            {
                Creature target = enemies.TakeRandom(1,base.Owner.RunState.Rng.Shuffle).FirstOrDefault();

                if (DynamicVars.TryGetValue(DazeVarName, out DynamicVar daze))
                    await MiyabiCombatService.AddDaze(target, daze, base.Owner.Creature);
                if (DynamicVars.TryGetValue(AnomalyBuildupVarName, out DynamicVar anomalyBuildup))
                    await MiyabiCombatService.AddAnoBuildup(target, anomalyBuildup.IntValue, base.Owner.Creature, this, choiceContext);

                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .Execute(choiceContext);

                if (target.IsDead) 
                {
                    enemies = base.Owner.Creature.CombatState.Enemies.Where(c => c.IsAlive);
                    continue; 
                }
               
            }

            if (base.CheckSupportCost(1) != 0)
            {
                if (DynamicVars.TryGetValue("RepeatCount", out DynamicVar repeat)) repeat.BaseValue += 1;
                await CostSupporPoint(1);
            }
            
        }

        public override Task AfterCombatEnd(CombatRoom room)
        {
            if(base.IsUpgraded)
                DynamicVars["RepeatCount"].BaseValue = 2;
            else
                DynamicVars["RepeatCount"].BaseValue = 1;
            return base.AfterCombatEnd(room);
        }

        protected override void OnUpgrade()
        {
            if (DynamicVars.TryGetValue("RepeatCount", out DynamicVar repeat)) repeat.BaseValue += 1;
        }
    }
}
