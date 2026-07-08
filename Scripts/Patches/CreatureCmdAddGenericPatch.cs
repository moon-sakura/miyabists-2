using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// Patch CreatureCmd.Add&lt;T&gt;，将硬编码的 CombatSide.Enemy 改为 combatState.CurrentSide。
    ///
    /// 源码：
    ///   Creature creature = combatState.CreateCreature(
    ///       ModelDb.Monster&lt;T&gt;().ToMutable(),
    ///       CombatSide.Enemy,  // ← 改为 CurrentSide
    ///       slotName);
    /// </summary>
    //[HarmonyPatch]
    //public static class CreatureCmdAddGenericPatch
    //{
    //    [HarmonyTargetMethods]
    //    public static IEnumerable<MethodBase> TargetMethods()
    //    {
    //        foreach (var method in typeof(CreatureCmd).GetMethods(
    //            BindingFlags.Public | BindingFlags.Static))
    //        {
    //            if (method.Name == "Add" && method.IsGenericMethod)
    //                yield return method;
    //        }
    //    }

    //    [HarmonyPrefix]
    //    public static bool Prefix(
    //        ICombatState combatState,
    //        string? slotName,
    //        ref Task<Creature> __result,
    //        MethodBase __originalMethod)
    //    {
    //        // 从原始泛型方法中提取 T
    //        Type monsterType = __originalMethod.GetGenericArguments()[0];

    //        // 异步执行替换逻辑，跳过原方法
    //        __result = AddWithCurrentSide(combatState, monsterType, slotName);
    //        return false;
    //    }

    //    private static async Task<Creature> AddWithCurrentSide(
    //        ICombatState combatState,
    //        Type monsterType,
    //        string? slotName)
    //    {
    //        // ModelDb.Monster<T>() 反射
    //        MethodInfo? monsterMethod = typeof(ModelDb).GetMethods()
    //            .FirstOrDefault(m => m.Name == "Monster"
    //                && m.IsGenericMethod
    //                && m.GetParameters().Length == 0);

    //        if (monsterMethod == null)
    //            throw new InvalidOperationException("ModelDb.Monster<T>() not found");

    //        MethodInfo genericMethod = monsterMethod.MakeGenericMethod(monsterType);
    //        MonsterModel monster = (MonsterModel)genericMethod.Invoke(null, null)!;

    //        // 关键改动：CombatSide.Enemy → combatState.CurrentSide
    //        Creature creature = combatState.CreateCreature(
    //            monster.ToMutable(),
    //            combatState.CurrentSide,
    //            slotName);

    //        // 调用非泛型 CreatureCmd.Add(Creature) 完成注册
    //        await CreatureCmd.Add(creature);
    //        return creature;
    //    }
    //}
}
