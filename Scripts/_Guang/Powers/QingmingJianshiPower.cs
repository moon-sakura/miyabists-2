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

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 青溟剑势 - 叶瞬光专属资源
    /// 本身只担任计数器，消耗逻辑参考 SupportPointPower。
    /// </summary>
    internal class QingmingJianshiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/qingmingJianshi.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        // 是否有免费使用次数（对应 SupportPointPower.isSupportFree）
        private bool isJianshiFree = false;

        /// <summary>0 = 点不起，1 = 正常扣除，2 = 免费</summary>
        public int CanUsePoint(int a)
        {
            if (a <= DisplayAmount) return 1;
            if (isJianshiFree) return 2;
            return 0;
        }

        public void SetFree() => isJianshiFree = true;
        public void ResetFree() => isJianshiFree = false;

        // ========== 青溟剑势归 0 ==========
        // 归 0 时若还处于明心镜状态，《明心镜·归尘》自动加入手卡。
        // 剑势归 0 可能表现为「层数变成 0」也可能表现为「能力被移除」，两条路都接上。
        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power != this) return;
            if (Amount > 0) return;

            await NotifyJianshiReachedZero();
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            await NotifyJianshiReachedZero();
        }

        private async Task NotifyJianshiReachedZero()
        {
            // 只在明心镜状态下才需要归尘；不在状态里说明归 0 与明心镜无关
            if (!Owner.HasPower<MingxinJingPower>()) return;
            await Owner.GetPower<MingxinJingPower>().AddGuiChenIfNeeded();
        }
    }
}
