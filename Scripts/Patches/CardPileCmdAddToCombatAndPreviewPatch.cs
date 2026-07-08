using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 防止给非 Player 的 Creature 塞牌（如 Dazed 等状态牌）造成逻辑错误。
    ///
    /// 原设想是 patch CardPileCmd.AddToCombatAndPreview&lt;T&gt;(IEnumerable&lt;Creature&gt; targets, ...)
    /// 在入口处检查 targets，但该方法是泛型 async 方法，Harmony 的 DynamicMethod wrapper 无法处理
    /// 编译器生成的 state machine struct 携带的泛型参数 T（MonoMod ImportGenericParameter NotSupportedException）。
    ///
    /// 替代方案：patch 下游的非泛型方法 AddGeneratedCardToCombat，通过 cardModel 的 Owner 判断
    /// 卡牌是否属于 Player。若卡牌最终归属不是 Player，则截断。
    ///
    /// 原方法签名（非泛型）：
    ///   Task&lt;CardPileAddResult&gt; AddGeneratedCardToCombat(CardModel cardModel, PileType pileType, Player? creator, CardPilePosition position = ...)
    /// </summary>
    //[HarmonyPatch]
    //public static class CardPileCmdAddToCombatAndPreviewPatch
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        MethodInfo? method = typeof(CardPileCmd)
    //            .GetMethods(BindingFlags.Public | BindingFlags.Static)
    //            .FirstOrDefault(m =>
    //            {
    //                if (m.Name != nameof(CardPileCmd.AddToCombatAndPreview))
    //                    return false;

    //                if (!m.IsGenericMethodDefinition)
    //                    return false;

    //                ParameterInfo[] parameters = m.GetParameters();

    //                return parameters.Length == 5
    //                    && parameters[0].ParameterType == typeof(Creature)
    //                    && parameters[1].ParameterType == typeof(PileType)
    //                    && parameters[2].ParameterType == typeof(int)
    //                    && parameters[3].ParameterType == typeof(Player)
    //                    && parameters[4].ParameterType == typeof(CardPilePosition);
    //            });

    //        if (method == null)
    //        {
    //            throw new MissingMethodException(
    //                "Could not find CardPileCmd.AddToCombatAndPreview<T>(Creature, PileType, int, Player, CardPilePosition)"
    //            );
    //        }

    //        return method;
    //    }

    //    static bool Prefix(Creature target, ref Task __result)
    //    {
    //        if (!target.IsPlayer)
    //        {
    //            __result = Task.CompletedTask;
    //            return false; // 跳过原方法
    //        }

    //        return true;
    //    }
    //}
}
