using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TrainEatOverPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (card.Type != MegaCrit.Sts2.Core.Entities.Cards.CardType.Attack)
            {
                return false;
            }
            // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }

        // 核心逻辑 2：修改星能/特殊消耗（保持逻辑一致）
        public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (card.Type != MegaCrit.Sts2.Core.Entities.Cards.CardType.Attack)
            {
                return false;
            }
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }
    }
}
