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
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
namespace Miyabists2.Scripts.Powers
{
    internal class AttributeAnomalyPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/powers/FrostBuild.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        // 效果 1：受到伤害 +20%
        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (target == base.Owner && isValidMove)
            {
                return 1.20m;
            }
            return 1m;
        }

        // 效果 2：添加时逻辑（检查重复以触发紊乱）
        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            // 如果目标已经拥有此能力，触发紊乱
            if (base.Owner.HasPower<AttributeAnomalyPower>())
            {
                await PowerCmd.Apply<DisorderPower>(base.Owner, 1, applier, cardSource);
                await PowerCmd.Remove(this);
            }
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if(base.Amount >= 2)
            {
                await PowerCmd.Apply<DisorderPower>(base.Owner, 1, null, null);
                //await PowerCmd.Remove(this);
                Amount = 0;
            }
        }

        // 效果 3：每回合结束受到 5 点伤害
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                // 造成 5 点固定伤害（Unpowered 确保不被自己再次增幅）
                await CreatureCmd.Damage(choiceContext, base.Owner, 5m, ValueProp.Unpowered, (Creature)null);
            }
        }

    }
}
