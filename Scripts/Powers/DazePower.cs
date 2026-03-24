using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Runtime.InteropServices;


namespace Miyabists2.Scripts.Powers
{
    internal class DazePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public override int DisplayAmount => Amount - 1;

        private int _max = 100;

        public string BigIconPath => "res://images/powers/FrostBuild.png";
        public string BigBetaIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomPackedIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomBigIconPath => "res://images/powers/FrostBuild.png";

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Owner.HasPower<BreakPower>()) 
            {
                await PowerCmd.SetAmount<DazePower>(base.Owner, 1m, null, null);
                return; 
            } // 如果已经有BreakPower，不再触发
            await CheckDazeTrigger(base.Owner);
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.HasPower<BreakPower>())
            {
                await PowerCmd.SetAmount<DazePower>(base.Owner, 1m, null, null);
                return;
            } // 如果已经有BreakPower，不再触发
            await CheckDazeTrigger(base.Owner);
        }

        public async Task CheckDazeTrigger(Creature target)
        {
            if (this.DisplayAmount >= _max)
            {
                await PowerCmd.Apply<BreakPower>(target, 1m, null, null);
                await PowerCmd.SetAmount<DazePower>(base.Owner, 1m, null, null);
            }
        }
    }
}