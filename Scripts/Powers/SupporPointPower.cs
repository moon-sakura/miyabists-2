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
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class SupportPointPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        private const int MaxPoints = 6; // 上限为 6
        private bool isSupportFree = false;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        // 核心：当层数改变时，强制限制在 0 到 6 之间
        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Amount > MaxPoints)
            {
                // 如果超过 6，则设回 6
                Amount = 6;
            }
            else if (base.Amount < 0)
            {
                // 确保不为负数
                Amount = 0;
            }
        }
    }
}
