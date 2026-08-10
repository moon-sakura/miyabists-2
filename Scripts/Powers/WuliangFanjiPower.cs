using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    /// 无量反击能力：本回合每完全格挡一次攻击，下回合获得1点能量
    /// Amount 记录完全格挡攻击的次数
    /// </summary>
    internal class WuliangFanjiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/wuliangFanji.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("ParryCount", 0),
            new EnergyVar(1),
        ];

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner) return;

            // 完全格挡一次攻击
            if (result.WasFullyBlocked)
            {
                DynamicVars["ParryCount"].BaseValue += 1;
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            // 下回合获得1点能量 × 完全格挡次数
            if (DynamicVars["ParryCount"].BaseValue > 0)
            {
                await PlayerCmd.GainEnergy(DynamicVars["ParryCount"].BaseValue * Amount, player);
            }

            // 结算完毕后移除
            await PowerCmd.Remove(this);
        }
    }
}
