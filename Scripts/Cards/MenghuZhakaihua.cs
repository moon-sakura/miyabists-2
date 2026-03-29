using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class MenghuZhakaihua: MiyabiPartnerCardBase
    {
        public MenghuZhakaihua() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Skill) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            //new DamageVar(5, ValueProp.Move),
            new DynamicVar(DazeVarName, 20),
            new DynamicVar("Decible",5)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            //if (base.DynamicVars.Damage.BaseValue > 0)
            //{
            //    await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            //        .FromCard(this)
            //        .Targeting(cardPlay.Target)
            //        .Execute(choiceContext);
            //}

            if (base.CheckSupportCost(1) != 0)
            {
                if (base.DynamicVars.TryGetValue("Decible", out DynamicVar d))
                    await MiyabiCombatService.AddDecible(base.Owner, d.IntValue);
                await CostSupporPoint(1);
            }
        }


        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(5);
            if (base.DynamicVars.TryGetValue("Decible", out DynamicVar d)) d.UpgradeValueBy(2);
        }
    }
}
