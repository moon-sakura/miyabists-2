using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    /// <summary>
    /// 芳菲之邀能力：每回合开始时对所有敌人造成6点伤害，并施加1层流明
    /// </summary>
    internal class FangfeiZhiyaoPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/fangfeiZhiyao.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            // 对所有敌人造成6点伤害
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                await CreatureCmd.Damage(choiceContext, enemy, 6m,
                    ValueProp.Unpowered, Owner);
            }

            // 对所有敌人施加1层流明
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<LiumingPower>(choiceContext, enemy, 1m, Owner, null);
            }
        }
    }
}
