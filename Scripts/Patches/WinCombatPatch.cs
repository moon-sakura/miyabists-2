using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Patches
{
    [HarmonyPatch(typeof(ProgressSaveManager))]
    public class FixEpochSubMethodsPatch
    {
        // 1. 拦截 Boss 战后的 Epoch 解锁
        [HarmonyPatch("ObtainCharUnlockEpoch")]
        [HarmonyPrefix]
        static bool PrefixObtain(Player localPlayer)
        {
            // 如果是你的 Mod 角色，直接拒绝执行这个方法，防止去查不存在的 Epoch ID
            if (IsModCharacter(localPlayer)) return false;
            return true;
        }

        // 2. 拦截 15 个 Boss 击杀的 Epoch 检查
        [HarmonyPatch("CheckFifteenBossesDefeatedEpoch")]
        [HarmonyPrefix]
        static bool PrefixFifteenBoss(Player localPlayer)
        {
            if (IsModCharacter(localPlayer)) return false;
            return true;
        }

        // 3. 拦截 15 个精英击杀的 Epoch 检查
        [HarmonyPatch("CheckFifteenElitesDefeatedEpoch")]
        [HarmonyPrefix]
        static bool PrefixFifteenElite(Player localPlayer)
        {
            if (IsModCharacter(localPlayer)) return false;
            return true;
        }

        // 💡 提取一个通用的判断工具，管它后面带不带数字 2、3
        private static bool IsModCharacter(Player player)
        {
            if (player == null) return false;

            return player.Character is Miyabi ||
                   player.Character is Yixuan;
        }
    }
}
