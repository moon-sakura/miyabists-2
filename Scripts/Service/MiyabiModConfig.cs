using BaseLib.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    [ConfigHoverTipsByDefault]
    public sealed class MiyabiModConfig:SimpleModConfig
    {
        [ConfigSection("CombatConfig")] // 创建一个战斗设置分组
        //[ConfigHoverTip]
        public static bool MiyabiEnemiesStronger { get; set; } = false;

        // 2. 限制造成伤害 (使用百分比滑块展示，范围 0% 到 100%)
        [ConfigSlider(0.3, 1.5, 0.05, Format = "0:0.#}x")]
        [ConfigHoverTip]
        public static double DamageDealtLimit { get; set; } = 1.0;

        // 3. 受到更多伤害 (范围 1x 到 5x)
        [ConfigSlider(0.5, 3.0, 0.1, Format = "{0:0.#}x")]
        [ConfigHoverTip]
        public static double DamageTakenMultiplier { get; set; } = 1.0;

        public static bool ChangeToAllPlayers { get; set; } = false;
    }
}
