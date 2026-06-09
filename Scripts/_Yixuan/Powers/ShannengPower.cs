using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    //[RegisterPower]
    internal class ShannengPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        private const int MaxPoints = 151; // 上限为 150

        public override int DisplayAmount => Amount - 1;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/shanneng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Amount > MaxPoints)
            {
                //Amount = MaxPoints;
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                // 确保不为负数
                //Amount = 1;
                SetAmount(1);
            }
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power != this) return;
            if (base.Amount > MaxPoints)
            {
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                // 确保不为负数
                SetAmount(1);
            }
        }


        public bool CanUseShanneng(int a)
        {
            if (a <= DisplayAmount) return true;
            return false;
        }
    }
}
