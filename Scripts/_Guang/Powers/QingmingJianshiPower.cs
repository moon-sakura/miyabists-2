using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
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

        // ========== 待实现 ==========
        // TODO: 青溟剑势本身不带效果。
        // 后续若要附带效果（例如每层增伤、达到阈值触发等），在此处补充对应回调即可。
    }
}
