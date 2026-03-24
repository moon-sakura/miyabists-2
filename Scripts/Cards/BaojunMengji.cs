using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class BaojunMengji : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public BaojunMengji() : base(3, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(20, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new BlockVar(12,ValueProp.Move),
            new DynamicVar(ParryVarName, 2)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            base.OnPlay(choiceContext, cardPlay);
            if (base.CheckSupportCost(2) != 0) 
            {
                await PowerCmd.Apply<PlatingPower>(base.Owner.Creature, 4, base.Owner.Creature, this);
                await CostSupporPoint(2);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(6);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
            DynamicVars.Block.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p)) p.UpgradeValueBy(1);

        }
    }
}

