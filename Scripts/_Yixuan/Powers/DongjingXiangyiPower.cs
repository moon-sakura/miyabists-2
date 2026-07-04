using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Cards;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    /// <summary>
    /// 动静相宜：使用青溟云影后消耗所有闪能恢复百分比闪能；使用符法千重后获得喧响值
    /// </summary>
    internal class DongjingXiangyiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/char/common.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("ShannengRecoverPercent", 50),
            new DynamicVar("DecibelAmount", 10),
        ];

        public void SetAmounts(int recoverPercent, int decibelAmount)
        {
            DynamicVars["ShannengRecoverPercent"].BaseValue = recoverPercent;
            DynamicVars["DecibelAmount"].BaseValue = decibelAmount;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;

            if (cardPlay.Card is QingmingYunying)
            {
                // 消耗所有闪能
                var shannengPower = Owner.GetPower<ShannengPower>();
                if (shannengPower != null)
                {
                    int currentShanneng = shannengPower.DisplayAmount;
                    if (currentShanneng > 0)
                    {
                        await shannengPower.UseShanneng(choiceContext, currentShanneng);

                        // 恢复消耗闪能的百分比
                        int recoverAmount = currentShanneng * DynamicVars["ShannengRecoverPercent"].IntValue / 100;
                        if (recoverAmount > 0)
                        {
                            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner, recoverAmount, Owner, null);
                        }
                    }
                }
            }
            else if (cardPlay.Card is FufaQianchong)
            {
                // 获得喧响值
                await MiyabiCombatService.AddDecible(Owner.Player, DynamicVars["DecibelAmount"].IntValue);
            }
        }
    }
}
