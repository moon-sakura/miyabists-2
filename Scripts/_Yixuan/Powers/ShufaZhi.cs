using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Combat.HealthBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class ShufaZhi : ModPowerTemplate, IHealthBarForecastSource
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/shanneng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
        {
            return HealthBarForecasts.Single(
                context.Creature.GetPowerAmount<ShufaZhi>(), // 展示的数量（例如如果你的能力有2倍效果可以乘2）
                new Color("8B7539"), // 颜色
                HealthBarForecastGrowthDirection.FromLeft // 从左边开始延伸还是右边开始
                                                          // 0, // 顺序，越大越远离血条边缘，默认0
                                                          // PreloadManager.Cache.GetMaterial("res://xxx.tres") // 如果需要自定义材质
            );
        }
    }
}
