using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
namespace Miyabists2.Scripts.Powers
{
    internal class DongFengSpPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = !props.HasFlag(ValueProp.Unpowered);
            if (dealer == base.Owner && isValidMove)
            {
                return 1.5m;
            }
            return 1m;
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = !props.HasFlag(ValueProp.Unpowered);
            if (dealer == base.Owner && result.TotalDamage > 0 && isValidMove)
            {
                // 触发一次后移除
                await PowerCmd.Remove(this);
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;
            await PowerCmd.Remove(this);
        }
    }
}
