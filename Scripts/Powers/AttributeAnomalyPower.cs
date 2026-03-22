using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
namespace Miyabists2.Scripts.Powers
{
    internal class AttributeAnomalyPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/powers/FrostBuild.png";
        public string BigBetaIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomPackedIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomBigIconPath => "res://images/powers/FrostBuild.png";

    }
}
