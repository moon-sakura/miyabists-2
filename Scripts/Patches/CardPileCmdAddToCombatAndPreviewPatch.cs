using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 拦截 CardPileCmd.AddToCombatAndPreview&lt;T&gt;，若 targets 包含非 Player 则截断。
    ///
    /// 该方法本身是泛型 async 方法，Harmony 的 Prefix/Postfix/Transpiler 全部在
    /// MethodCreator.CreateReplacement() → DeclareOriginalLocalVariables() 阶段失败：
    ///   state machine struct &lt;d__X&gt;&lt;T&gt; 的 local variable 类型携带泛型参数 T，
    ///   MonoMod 无法将 T 导入到 DynamicMethod wrapper → ImportGenericParameter NotSupportedException。
    ///
    /// 解决方案：不 patch 方法本体，而 patch state machine 的 MoveNext() 方法。
    ///   MoveNext 本身不是泛型方法（是泛型 struct 上的实例方法），
    ///   local variables 全部是具体类型（int、IEnumerator、TaskAwaiter 等），
    ///   Harmony 的 __instance 参数走 object 类型，不需要导入泛型参数。
    ///   通过反射读取 state machine 的 targets 字段来判断是否放行。
    /// </summary>
    //[HarmonyPatch]
    //public static class CardPileCmdAddToCombatAndPreviewPatch
    //{
    //    /// <summary>
    //    /// 找到 state machine struct 的 MoveNext 方法作为 patch 目标。
    //    /// </summary>
    //    [HarmonyTargetMethod]
    //    public static MethodBase TargetMethod()
    //    {
    //        // 1. 找到 CardPileCmd.AddToCombatAndPreview<T> 的 IEnumerable<Creature> 重载
    //        MethodInfo? originalMethod = null;
    //        foreach (var method in typeof(CardPileCmd).GetMethods(
    //            BindingFlags.Public | BindingFlags.Static))
    //        {
    //            if (method.Name == "AddToCombatAndPreview"
    //                && method.IsGenericMethodDefinition
    //                && method.GetParameters().Length == 5
    //                && method.GetParameters()[0].ParameterType == typeof(IEnumerable<Creature>))
    //            {
    //                originalMethod = method;
    //                break;
    //            }
    //        }

    //        if (originalMethod == null)
    //            return null!;

    //        // 2. 从 [AsyncStateMachine] attribute 获取 state machine 类型
    //        var attr = originalMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
    //        if (attr == null)
    //            return null!;

    //        Type stateMachineType = attr.StateMachineType;
    //        // stateMachineType 是 open generic type，例如 <AddToCombatAndPreview>d__X<T>

    //        // 3. 获取 MoveNext 方法（实现 IAsyncStateMachine.MoveNext）
    //        var moveNext = stateMachineType.GetMethod("MoveNext",
    //            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //        // MoveNext 无泛型参数，local variables 全部是具体类型 → 可以被 Harmony patch

    //        return moveNext!;
    //    }

    //    /// <summary>
    //    /// Prefix：通过反射读取 state machine 的 targets 字段判断是否放行。
    //    /// __instance 即 state machine struct 实例。
    //    /// </summary>
    //    [HarmonyPrefix]
    //    public static bool Prefix(object __instance)
    //    {
    //        if (__instance == null)
    //            return true;

    //        // 在 state machine 的所有字段中查找类型为 IEnumerable<Creature> 的字段
    //        var targetsField = __instance.GetType()
    //            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    //            .FirstOrDefault(f => f.FieldType == typeof(IEnumerable<Creature>));

    //        if (targetsField == null)
    //            return true; // 找不到 targets 字段，保守放行

    //        var targets = (IEnumerable<Creature>?)targetsField.GetValue(__instance);
    //        if (targets == null)
    //            return true;

    //        // 有任何非 Player 目标 → 截断
    //        if (!targets.All(c => c == null || c.IsPlayer))
    //        {
    //            return false; // 跳过 MoveNext → 方法直接返回（state = -2, completed）
    //        }

    //        return true; // 放行
    //    }
    //}
}
