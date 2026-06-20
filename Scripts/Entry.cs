using BaseLib.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Managers;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using System.Reflection;

namespace Miyabists2.Scripts;

// 必须要加的属性，用于注册Mod。字符串和初始化函数命名一致。
[ModInitializer("Init")]
public class Entry
{
    public const string ModId = "Miyabists2";
    public static readonly MegaCrit.Sts2.Core.Logging.Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    // 初始化函数
    public static void Init()
    {

        MiyabiSkinManager.PreSkinRegister();

        var config = new MiyabiModConfig();
        ModConfigRegistry.Register(ModId, config);

        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        var harmony = new Harmony("com.YueSakura.Miyabi");
        harmony.PatchAll();


        MiyabiSkinManager.RegisterCombatSkin("银庭花信", "res://scenes/miyabi_yinxing_char.tscn", "res://images/charui/miyabi_yinxing_char75.png");
        MiyabiSkinManager.RegisterShopSkin("银庭花信", "res://scenes/miyabi_yinxing_shop.tscn", "res://images/charui/miyabi_yinxing_char75.png");

        MiyabiSkinManager.RegisterCombatSkinYixuan("墨形影踪", "res://scenes/_Yixuan/yixuan_moxing_combat.tscn", "res://images/_YiXuan/char/yixua_moxing.png");
        MiyabiSkinManager.RegisterShopSkinYixuan("墨形影踪", "res://scenes/_Yixuan/yixuan_moxing_shop.tscn", "res://images/_YiXuan/char/yixua_moxing.png");
        //Log.Debug("星见雅MOD加载完成");
    }

    //[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
    //internal static class TouchOfOrobasGetUpgradedStarterRelicPatch
    //{
    //    private static bool Prefix(TouchOfOrobas __instance, RelicModel starterRelic, ref RelicModel __result)
    //    {
    //        if (starterRelic is SwordNotailRelic)
    //        {
    //            __result = ModelDb.Relic<NoTailFullRelic>();
    //            return false;
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(LocTable))]
    //public class LocTableAccessTrackerPatch
    //{
    //    // 🎯 目标方法：LocTable 内部的 HasEntry
    //    [HarmonyPatch(nameof(LocTable.HasEntry), new Type[] { typeof(string) })]
    //    [HarmonyPrefix]
    //    public static void Prefix(LocTable __instance, string key)
    //    {
    //        // __instance.Name 可以获取当前正在被查询的表名（比如 "settings_ui"）
    //        // 过滤我们关心的设置表，以及包含你的 mod 前缀或 slot 的请求
    //        //if (__instance.Name == "settings_ui")
    //        {
    //            GD.Print($"[MiyabiTracker] UI正在查询表 [settings_ui] 的 Key: \"{key}\"");

    //            // 顺便在这里检测一下，此时这个表里到底有没有这个 Key
    //            // 如果你发现 UI 查了 "miyabi_slot_1.title"，但紧接着打印出“未找到”，说明你的注入时机或者 Key 拼错了。
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Miyabi), "get_CustomVisualsPath")]
    //public class MiyabiSkinPatch
    //{
    //    public static bool Prefix(ref string __result)
    //    {
    //        __result = "res://scenes/monsters/yixuan_enemy.tscn";

    //        return false;
    //    }
    //}


    //[HarmonyPatch(typeof(CharacterModel), "EnergyCounterPath", MethodType.Getter)]
    //public static class EnergyIconPatch
    //{
    //    // 这个方法会在游戏读取能量球路径时触发
    //    public static void Postfix(CharacterModel __instance, ref string __result)
    //    {
    //        // 判定：如果是你的角色，就强行把返回值改成机器人的
    //        // 请将 Miyabi 替换为你角色类的真实名称
    //        if (__instance is Miyabi)
    //        {
    //            __result = SceneHelper.GetScenePath("combat/energy_counters/defect_energy_counter.scn");
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(TheArchitect), "WinRun")]
    //internal static class WatcherArchitectWinRunPatch
    //{
    //    private static bool Prefix(TheArchitect __instance, ref Task __result)
    //    {
    //        FieldInfo fieldInfo = AccessTools.Field(typeof(TheArchitect), "_dialogue");
    //        if (((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) != null)
    //        {
    //            return true;
    //        }

    //        if (LocalContext.IsMe(__instance.Owner))
    //        {
    //            RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
    //        }

    //        __result = Task.CompletedTask;
    //        return false;
    //    }
    //}
}
