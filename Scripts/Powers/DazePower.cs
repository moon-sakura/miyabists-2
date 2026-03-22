using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using System.Runtime.InteropServices;


namespace Miyabists2.Scripts.Powers
{
    internal class DazePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/powers/FrostBuild.png";
        public string BigBetaIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomPackedIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomBigIconPath => "res://images/powers/FrostBuild.png";

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Owner.HasPower<BreakPower>()) 
            {
                await PowerCmd.Remove(this);
                return; 
            } // 如果已经有BreakPower，不再触发
            await CheckDazeTrigger(base.Owner);
        }

        public async Task CheckDazeTrigger(Creature target)
        {
            if (this.Amount >= 100)
            {
                await PowerCmd.Apply<BreakPower>(target, 1m, null, null);
                await PowerCmd.Remove(this);
            }
        }
    }
}