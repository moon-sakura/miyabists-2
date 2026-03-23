using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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

        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (!base.Owner.Creature.HasPower<SlipperyPower>())
            {
                return Task.CompletedTask;
            }
            ReduceCostBy(1);
            return Task.CompletedTask;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            DynamicVars.Cards.UpgradeValueBy(1);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }

        private void ReduceCostBy(int amount)
        {
            base.EnergyCost.AddThisTurn(-amount);
        }
    }
}
