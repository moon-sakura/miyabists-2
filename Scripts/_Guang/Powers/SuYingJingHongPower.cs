using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 溯影惊鸿能力：每次使用伙伴卡获得1点青溟剑势
    /// </summary>
    internal class SuYingJingHongPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/suYingJingHong.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;

            // 使用伙伴卡时获得青溟剑势
            // 说明：伙伴卡通过 Friends 关键字标记
            if (cardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends))
            {
                await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner,
                    Amount, Owner, null);
            }
        }
    }
}
