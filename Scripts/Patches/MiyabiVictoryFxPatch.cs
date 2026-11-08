using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using Miyabists2.Scripts.Service;
using System;
using System.Reflection;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 战斗胜利判定成立、进入收尾流程的那一刻触发一次 WIPEOUT 滤镜。
    ///
    /// CombatManager 有两个 EndCombatInternal：
    ///   - 无参版本：测试用入口
    ///   - EndCombatInternal(CombatTurnState)：正常战斗胜利走这里（CheckWinCondition 调用）
    /// 这里精确匹配带参数的私有版本；它只在“敌人全灭、且没有阻止结束的钩子”时被调用，
    /// 玩家死亡的路径走 PendingLoss，不会触发。
    ///
    /// CombatTurnState 是 internal 类型，没法直接写 typeof，所以通过反射拿。
    /// 注意：这是 async 方法，Harmony 的 Prefix 在状态机启动前同步执行，
    /// 正好是最后一击结算完、胜利收尾开始的瞬间。
    /// </summary>
    [HarmonyPatch]
    public static class MiyabiVictoryFxPatch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            Type? turnStateType = typeof(CombatManager).Assembly
                .GetType("MegaCrit.Sts2.Core.Combat.CombatTurnState");

            if (turnStateType != null)
            {
                MethodInfo? method = AccessTools.Method(
                    typeof(CombatManager), "EndCombatInternal", new[] { turnStateType });

                if (method != null)
                    return method;

                GD.PrintErr("[MiyabiVictoryFx] 找不到 EndCombatInternal(CombatTurnState)，特效不会触发。");
            }
            else
            {
                GD.PrintErr("[MiyabiVictoryFx] 找不到 CombatTurnState 类型，特效不会触发。");
            }

            // 兜底：返回无参版本，避免 TargetMethod 为 null 导致 PatchAll 抛异常。
            // 无参版本在正常战斗流程里不会走到，所以特效会失效，但不会影响 Mod 加载。
            return AccessTools.Method(typeof(CombatManager), "EndCombatInternal", Type.EmptyTypes)
                ?? throw new MissingMethodException("CombatManager.EndCombatInternal");
        }

        [HarmonyPrefix]
        public static void Prefix()
        {
            MiyabiVictoryFxService.Play();
        }
    }
}
