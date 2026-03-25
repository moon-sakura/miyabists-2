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
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostPower: CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        //public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        //{
        //    if (base.Owner.GetPowerAmount<FrostBuildPower>() >= 101)
        //    {
        //        await PowerCmd.SetAmount<FrostBuildPower>(base.Owner, 1, null, null);
        //    }

        //    if (base.Owner.HasPower<FrostFirePower>())
        //    {
        //        //造成冰焰层数*1.5点伤害，清除冰焰
        //        int fireAmount = base.Owner.GetPowerAmount<FrostFirePower>();

        //        await CreatureCmd.Damage(null, base.Owner, fireAmount * 1.5m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, base.Owner);

        //        //await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        //        //.Targeting(base.Owner)
        //        //.Execute(null);

        //        if (!MiyabiCombatService.ShouldKeepFrostFire())
        //            await PowerCmd.Remove<FrostFirePower>(base.Owner);
        //    }
        //    //添加一次属性异常
        //    //await PowerCmd.Apply<AttributeAnomalyPower>(base.Owner, 1, null, null);
        //    //await PowerCmd.Remove(this);

        //    //await PowerCmd.TickDownDuration(this);
        //}

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (target == base.Owner && isValidMove)
            {
                return 1.20m;
            }
            return 1m;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                //持续一回合
                await PowerCmd.Remove(this);
            }
        }



    }
}
