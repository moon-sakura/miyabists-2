using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(power == this)
            {
                if(Amount >= 100)
                {
                    await PowerCmd.Apply<FumoPower>(choiceContext, Owner, 1, null, null);
                    SetAmount(Amount - 100);
                }
            }
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            int fumo = Owner.GetPowerAmount<FumoPower>();

            if (target == Owner && !props.HasFlag(ValueProp.Unpowered)
                && Amount*(Owner.MaxHp/100m) >= Owner.CurrentHp)
            {
                if (fumo > 0)
                {
                    return 2 + fumo;
                }
                return 2m;
            }

            if (dealer == Owner && !props.HasFlag(ValueProp.Unpowered)
                && Amount * (Owner.MaxHp / 100m) >= Owner.CurrentHp)
            {
                if (fumo > 0)
                {
                    return 0.8m - 0.2m * fumo > 0 ? 0.8m - 0.2m * fumo : 0.05m;
                }
                return 0.8m;
            }

            return 1m + fumo;
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
