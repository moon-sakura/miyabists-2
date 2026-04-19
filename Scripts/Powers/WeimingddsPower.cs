using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class WeimingddsPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/weimingDds.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Daze", 0),
        ];

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner) return;

            await MiyabiCombatService.AddHuaCiReward(target, dealer, choiceContext, 1);

            if (!result.WasFullyBlocked && dealer != base.Owner)
            {
                await MiyabiCombatService.DazeAddtoPlayer(choiceContext, target, DynamicVars["Daze"].IntValue);
            }

        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target == base.Owner && !base.Owner.HasPower<BreakPlayerPower>())
            {
                return 0.75m;
            }
            return 1m;
        }

        public void SetDazeAdd(int amount)
        {
            base.DynamicVars["Daze"].BaseValue = amount;
        }
    }
}
