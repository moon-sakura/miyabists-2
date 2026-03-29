using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    internal class BigEjizhan : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/bigEjizhan.png";
        public BigEjizhan() : base(2, CardRarity.Common, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(12, ValueProp.Move),
            new DynamicVar(DazeVarName, 8),
            new DynamicVar(AnomalyBuildupVarName,1),
            new DynamicVar("Strength",1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            if (base.CheckSupportCost(1) != 0 && base.DynamicVars.TryGetValue("Strength", out DynamicVar s))
            {
                s.BaseValue += 1;
                await CostSupporPoint(1);
            }

            if (base.DynamicVars.TryGetValue("Strength", out DynamicVar t))
                await PowerCmd.Apply<EjizhanPower>(base.Owner.Creature, t.BaseValue, Owner.Creature, this);
        }


        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out DynamicVar a)) a.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue("Strength", out DynamicVar s)) s.UpgradeValueBy(1);
        }
    }
}
