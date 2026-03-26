using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
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
    internal class BamianWeifeng : MiyabiPartnerCardBase
    {
        public BamianWeifeng() : base(2,CardRarity.Uncommon,TargetType.Self, CardType.Power) { }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<BamianwfPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            if (base.CheckSupportCost(1) != 0)
            {
                MiyabiCombatService.AddDecible(Owner, 3);
                await CostSupporPoint(1);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
