using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostPower: CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            //造成冰焰层数*1.5点伤害，清除冰焰
            int fireAmount = base.Owner.GetPowerAmount<FrostFirePower>();

            await CreatureCmd.Damage(null, base.Owner, fireAmount * 1.5m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, (Creature)null);

            if (!MiyabiCombatService.ShouldKeepFrostFire())
            {
                await PowerCmd.Remove<FrostFirePower>(base.Owner);
            }
            //添加一次属性异常
            await PowerCmd.Apply<AttributeAnomalyPower>(base.Owner,1,null,null);
            //await PowerCmd.Remove(this);
            Amount = 0;
        }

        
    
    }
}
