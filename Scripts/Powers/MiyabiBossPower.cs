using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Miyabists2.Scripts.Powers
{
    internal class MiyabiBossPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/miyabiFull.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(applier != base.Owner || !(power is BreakPlayerPower || power is DisorderPower) || amount < 1)
            {
                return;
            }

            if(power is BreakPlayerPower)
            {
                foreach(var powerItem in base.Owner.Powers)
                {
                    if(powerItem.Type == PowerType.Debuff) 
                    {
                        await PowerCmd.Remove(powerItem);
                    }
                }
            }

            if(power is DisorderPower)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 2m, base.Owner, null);
            }
        }
    }
}
