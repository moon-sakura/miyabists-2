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
    [HarmonyPatch(typeof(ProgressSaveManager), "UpdateAfterCombatWon")]
    public class FixRitsuLibDynamicIdCrashPatch
    {
        static bool Prefix(Player localPlayer)
        {
            if (localPlayer?.Character?.Id != null)
            {
                string charId = localPlayer.Character.Id.ToString();

                // 🎯 只要检测到是你的 Mod 前缀角色，直接跳过 Epoch 进度检查
                if (charId.StartsWith("MIYABISTS2_CHARACTER_MIYABI") ||
                    charId.StartsWith("MIYABISTS2_CHARACTER_YIXUAN"))
                {
                    // 返回 false 代表不执行原版的 Epoch 局外结算
                    // 这样游戏会顺利保存你当前的关卡、金币、血量，直接进入选牌结算，绝不卡死！
                    return false;
                }
            }
            return true; // 原版铁甲战士、静默猎手等正常放行
        }
    }
}
