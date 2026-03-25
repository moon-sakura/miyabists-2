using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostFirePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner)
            {
                return 1m;
            }
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (!isValidMove) return 1m;

            if(!cardSource.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang))
                return 1m;

            //烈霜伤害+10%（效果上限200%，层数无上限）
            decimal damageIncrease = base.Amount * 0.05m;
            if (damageIncrease > 0.8m)
            {
                damageIncrease = 0.8m;
            }

            return 1m + damageIncrease;
        }
    }
}
