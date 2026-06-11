using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class RongluShengwenPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/char/common.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        /// <summary>每次施加失衡时额外施加的失衡值</summary>
        public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
        {
            if(giver == base.Owner && amount > 0 && power is DazePower)
            {
                return 5m;
            }
            return 0m;
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(power is DazePower && applier == Owner && amount > 5)
            {
                await PowerCmd.Decrement(this);
            }
        }
    }
}
