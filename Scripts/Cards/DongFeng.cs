using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    internal class DongFeng: MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public DongFeng() : base(1, CardRarity.Common, TargetType.AnyEnemy, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new CardsVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckReduce();
        }

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            if (side == base.Owner.Creature.Side)
            {
                await CheckReduce();
            }
        }

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if(card == this)
            {
                await CheckReduce();
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            DynamicVars.Cards.UpgradeValueBy(1);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }

        private async Task CheckReduce()
        {
            if(base.Owner != null && base.Owner.Creature.IsAlive)
            {
                if (base.Owner.Creature.HasPower<SlipperyPower>())
                    SetCost(0);
                else
                    SetCost(1);
            }
        }

        private void ReduceCostBy(int amount)
        {
            base.EnergyCost.AddThisTurn(-amount);
        }

        private void SetCost(int a) 
        {
            base.EnergyCost.SetThisTurn(a);
        }

    }
}
