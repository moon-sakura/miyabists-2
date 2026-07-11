using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
    ///
    /// 泛型 async 方法的 Prefix/Postfix 不被 MonoMod 支持（ImportGenericParameter），
    /// 故使用 Transpiler 直接修改 IL。
    /// </summary>
    //[HarmonyPatch]
    //public static class CreatureCmdAddGenericPatch
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        return typeof(CreatureCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
    //            .First(m => m.Name == "Add" && m.IsGenericMethodDefinition);
    //    }

    //    [HarmonyTranspiler]
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var list = instructions.ToList();

    //        // 查找 state machine 中存储 ICombatState 参数的字段
    //        FieldInfo? combatStateField = null;
    //        foreach (var instr in list)
    //        {
    //            if (instr.opcode == OpCodes.Ldfld && instr.operand is FieldInfo fi
    //                && fi.FieldType == typeof(ICombatState))
    //            {
    //                combatStateField = fi;
    //                break;
    //            }
    //        }

    //        if (combatStateField == null)
    //            return list;

    //        // 查找 CreateCreature 方法
    //        var createCreatureMethod = AccessTools.Method(
    //            typeof(ICombatState), nameof(ICombatState.CreateCreature));
    //        if (createCreatureMethod == null)
    //            return list;

    //        // 手动遍历 IL：找到 ldc.i4 → callvirt CreateCreature 的模式
    //        // 模式：ldc.i4.X (CombatSide.Enemy) ... callvirt CreateCreature
    //        for (int i = 0; i < list.Count - 1; i++)
    //        {
    //            // 找 ldc.i4 (加载 CombatSide 枚举值)
    //            if (!IsLdcI4(list[i].opcode))
    //                continue;

    //            // 向后搜索，在同一条执行路径上找 CreateCreature 的调用
    //            // （中间可能穿插其他与 CreateCreature 参数无关的指令）
    //            for (int j = i + 1; j < list.Count && j < i + 20; j++)
    //            {
    //                if (list[j].opcode == OpCodes.Callvirt
    //                    && list[j].operand is MethodInfo mi
    //                    && mi.Name == "CreateCreature"
    //                    && mi.DeclaringType == typeof(ICombatState))
    //                {
    //                    // 找到了！替换 ldc.i4 为 combatState.CurrentSide
    //                    list[i] = new CodeInstruction(OpCodes.Ldarg_0);   // state machine this
    //                    list.Insert(i + 1, new CodeInstruction(OpCodes.Ldfld, combatStateField));
    //                    list.Insert(i + 2, new CodeInstruction(OpCodes.Callvirt,
    //                        AccessTools.PropertyGetter(typeof(ICombatState), nameof(ICombatState.CurrentSide))));
    //                    // 删除原来的两条指令（它们被上面三条替换了，实际删 1 增 3）
    //                    // list[i] 已被替换为 Ldarg_0，i+1,i+2 是新插入的
    //                    // 原来的 ldc.i4 已被覆盖，不需要额外删除
    //                    return list;
    //                }

    //                // 遇到分支/返回指令则停止向前搜索
    //                if (list[j].opcode == OpCodes.Ret
    //                    || list[j].opcode == OpCodes.Br
    //                    || list[j].opcode == OpCodes.Brtrue
    //                    || list[j].opcode == OpCodes.Brfalse)
    //                    break;
    //            }
    //        }

    //        return list;
    //    }

    //    private static bool IsLdcI4(OpCode opcode)
    //    {
    //        return opcode == OpCodes.Ldc_I4
    //            || opcode == OpCodes.Ldc_I4_0
    //            || opcode == OpCodes.Ldc_I4_1
    //            || opcode == OpCodes.Ldc_I4_2
    //            || opcode == OpCodes.Ldc_I4_3
    //            || opcode == OpCodes.Ldc_I4_4
    //            || opcode == OpCodes.Ldc_I4_5
    //            || opcode == OpCodes.Ldc_I4_6
    //            || opcode == OpCodes.Ldc_I4_7
    //            || opcode == OpCodes.Ldc_I4_8
    //            || opcode == OpCodes.Ldc_I4_M1
    //            || opcode == OpCodes.Ldc_I4_S;
    //    }
    //}
}
