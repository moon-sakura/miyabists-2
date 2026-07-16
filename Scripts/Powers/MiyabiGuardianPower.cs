using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using MinionLib.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class MiyabiGuardianPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public string BigIconPath => ImageHelper.GetImagePath("powers/" + "die_for_you_power.png");
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override Creature ModifyUnblockedDamageTarget(Creature target, decimal amount, ValueProp props, Creature? dealer)
        {
            if (base.Owner.Monster is not MinionModel)
            {
                return target;
            }

            if (target != base.Owner.PetOwner?.Creature)
            {
                bool flag = true;
                if (target.PetOwner == base.Owner.PetOwner && base.Owner.PetOwner != null && target.GetPower<MinionGuardianPower>() != null)
                {
                    IReadOnlyList<Creature> pets = target.PetOwner.PlayerCombatState.Pets;
                    if (pets.IndexOf(base.Owner) < pets.IndexOf(target) && dealer.IsEnemy)
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    return target;
                }
            }

            if (base.Owner.IsDead)
            {
                return target;
            }

            if (!props.HasFlag(ValueProp.Move) || props.HasFlag(ValueProp.Unpowered))
            {
                return target;
            }

            return base.Owner;
        }
    }
}
