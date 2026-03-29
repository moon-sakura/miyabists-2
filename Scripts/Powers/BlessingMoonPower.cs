using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class BlessingMoonPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override bool ShouldDieLate(Creature creature)
        {
            if (creature != base.Owner)
            {
                return true;
            }
            return false;
        }

        public override async Task AfterPreventingDeath(Creature creature)
        {
            Flash();
            decimal amount = Math.Max(1m, (decimal)creature.MaxHp * (Amount / 100m));
            await CreatureCmd.Heal(creature, amount);
            await PowerCmd.Remove(this);
        }

    }
}
