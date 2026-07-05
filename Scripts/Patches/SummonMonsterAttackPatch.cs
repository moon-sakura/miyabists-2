using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 当怪物作为玩家随从（Pet）执行攻击时，将目标重定向为"所有敌人"。
    ///
    /// 调用链：
    ///   DamageCmd.Attack(damage)   // ← 设置伤害，创建 AttackCommand
    ///     .FromMonster(this)       // ← Harmony 拦截点：设置 attacker + targeting
    ///     .WithAttackerFx(...)     // ← FX 配置
    ///     .Execute(null)
    ///
    /// 两个问题使 Postfix 不可行：
    /// 1. FromMonster 内部已设置 targeting，再调 TargetingAllOpponents → "Already set" 异常
    /// 2. TargetingAllOpponents 需要 Attacker 已设置才能确定"对手"，但 Prefix 时
    ///    FromMonster 还没跑，Attacker 为空 → "require an attacker" 异常
    ///
    /// 解决：Prefix 中手动设置 Attacker（通过反射），然后调用 TargetingAllOpponents，
    /// 最后 return false 跳过原始 FromMonster。
    /// </summary>
    [HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.FromMonster))]
    public static class SummonMonsterAttackPatch
    {
        private static readonly PropertyInfo AttackerProperty =
            typeof(AttackCommand).GetProperty("Attacker", BindingFlags.Public | BindingFlags.Instance);

        private static readonly FieldInfo AttackerAnimNameField =
            typeof(AttackCommand).GetField("_attackerAnimName", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo SourceTypeField =
            typeof(AttackCommand).GetField("_sourceType", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(
            AttackCommand __instance,
            MonsterModel monster,
            ref AttackCommand __result)
        {
            if (__instance == null || monster == null)
                return true;

            // MonsterModel.Creature 是反向引用，指向拥有此模型的生物实例
            Creature attacker = monster.Creature;
            if (attacker == null)
                return true;

            // 只处理"属于玩家的宠物"的情况
            if (attacker.PetOwner == null || !attacker.IsPet || !attacker.PetOwner.Creature.IsPlayer)
                return true;

            ICombatState combatState = attacker.PetOwner.Creature.CombatState;
            if (combatState == null)
                return true;

            // 手动复制 FromMonster 的初始化逻辑，但使用 pet owner 的 CombatState
            // 这样 TargetingAllOpponents 的"对手" = 宠物主人的对手 = 敌人
            AttackerProperty?.SetValue(__instance, attacker);
            AttackerAnimNameField?.SetValue(__instance, "Attack");
            SourceTypeField?.SetValue(__instance, 0); // SourceType.Monster = 0

            // TargetingAllOpponents 需要 Attacker 已设置才能确定"对手"
            __result = __instance.TargetingAllOpponents(combatState);

            // 跳过原始 FromMonster（避免 targeting 冲突）
            return false;
        }
    }
}
