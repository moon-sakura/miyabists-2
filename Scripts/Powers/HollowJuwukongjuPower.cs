using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class HollowJuwukongjuPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/QinShiNorm.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        {
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);

            if (dealer != Owner || target != Owner || !isValidMove)
                return 1m;

            if(dealer == Owner && target.IsEnemy)
                return 1m + 0.2m * Amount;

            if(dealer.IsEnemy && target == Owner 
                && (dealer.CombatState.RunState.CurrentRoom.RoomType == MegaCrit.Sts2.Core.Rooms.RoomType.Elite
                || dealer.CombatState.RunState.CurrentRoom.RoomType == MegaCrit.Sts2.Core.Rooms.RoomType.Boss))
                return 1m + 0.25m * Amount;

            return 1m;
        }
    }
}
