using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
	internal class HollowJishengyijunPower : ModPowerTemplate
	{
		public override PowerType Type => PowerType.Debuff;
		public override PowerStackType StackType => PowerStackType.Counter;
		public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
		public string BigIconPath => "res://images/powers/QinShiNorm.png";
		public string BigBetaIconPath => BigIconPath;
		public override string CustomIconPath => BigIconPath;
		public override string CustomBigIconPath => BigIconPath;

		public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
		{
			if(Owner.CurrentHp >= Owner.MaxHp / 2)
			{
				await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), base.Owner, Amount, base.Owner, null);
			}
			else
			{
				if (Owner.HasPower<WeakPower>())
				{
					await PowerCmd.Remove<WeakPower>(Owner);
				}
				await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, Amount, base.Owner, null);
				await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner, Amount, base.Owner, null);
			}
		}
	}
}
