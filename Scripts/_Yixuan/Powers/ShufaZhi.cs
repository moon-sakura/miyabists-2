using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
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
        public string BigIconPath => "res://images/_YiXuan/char/common.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target == Owner && !props.HasFlag(ValueProp.Unpowered)
                && Amount*(Owner.MaxHp/100m) >= Owner.CurrentHp)
            {
                return 2m;
            }

            if (dealer == Owner && !props.HasFlag(ValueProp.Unpowered)
                && Amount * (Owner.MaxHp / 100m) >= Owner.CurrentHp)
            {
                return 0.7m;
            }

            return 1m;
        }

        public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
        {
            return HealthBarForecasts.Single(
                (int)(Amount * (Owner.MaxHp / 100m)), // 展示的数量（例如如果你的能力有2倍效果可以乘2）
                new Color("FFD700"), // 颜色
                HealthBarForecastGrowthDirection.FromLeft // 从左边开始延伸还是右边开始
                                                          // 0, // 顺序，越大越远离血条边缘，默认0
                                                          // PreloadManager.Cache.GetMaterial("res://xxx.tres") // 如果需要自定义材质
            );
        }
    }
}
