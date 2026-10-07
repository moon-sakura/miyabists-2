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
    /// 目前仅担任计数器，不附带任何效果。
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

        // ========== 待实现 ==========
        // TODO: 青溟剑势目前只作为计数使用。
        // 后续若要附带效果（例如每层增伤、达到阈值触发等），在此处补充对应回调即可。
    }
}
