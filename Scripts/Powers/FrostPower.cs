
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostPower: ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/shuangZhuo.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<FrostBuildPower>()
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar ("DamageVuln", 20),
            new DynamicVar ("CanAdd",0),
        ];

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == this)
            {
                DynamicVars["DamageVuln"].BaseValue = amount * 20;
            }
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if(MiyabiFuncBase.GetRelic<ShuangyanLiezhuoRelic>(Owner.Player) != null)
            {
                DynamicVars["CanAdd"].BaseValue += 1;
            }
            return base.AfterApplied(applier, cardSource);
        }


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
            decimal damageMultiplier = 1m + 0.2m * Amount;
            if (target == base.Owner && isValidMove)
            {
                return damageMultiplier;
            }
            return 1m;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants  )
        {
            if (side == base.Owner.Side)
            {
                //持续一回合
                await PowerCmd.Remove(this);
            }
        }



    }
}
