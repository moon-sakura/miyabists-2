using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        public static bool FrostFireKeeped { get; set; } = false;

        // 获取当前状态（可选，直接访问属性也行）
        public static bool ShouldKeepFrostFire() => FrostFireKeeped;

        // 设置状态
        public static void SetShouldKeepFrostFire(bool value) => FrostFireKeeped = value;

    }
}
