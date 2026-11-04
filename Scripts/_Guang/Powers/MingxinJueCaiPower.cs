using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts._Guang.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 明心镜·决裁 - 由《决裁》的能力变更而来。
    /// 攻击时额外造成消耗堆中诅咒卡数量的伤害。
    /// 回合结束时移除。
    /// </summary>
    internal class MingxinJueCaiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        /// <summary>图标上显示当前消耗堆中的诅咒卡数量（也就是攻击的额外伤害）</summary>
        public override int DisplayAmount => CurseCountInExhaust();

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => JueCai.SharedArtPath;
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        /// <summary>消耗堆中的诅咒卡数量</summary>
        private int CurseCountInExhaust()
        {
            Player? player = Owner?.Player;
            if (player == null) return 0;

            return PileType.Exhaust.GetPile(player).Cards.Count(c => c.Type == CardType.Curse);
        }

        public override decimal ModifyDamageAdditive(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay)
        {
            if (dealer != Owner) return 0m;
            if (!props.IsPoweredAttack()) return 0m;

            return CurseCountInExhaust();
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }
    }
}
