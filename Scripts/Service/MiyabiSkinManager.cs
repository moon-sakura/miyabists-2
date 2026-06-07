using STS2RitsuLib;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using Miyabists2.Scripts.Char;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.RestSite;

namespace Miyabists2.Scripts.Service
{
    public static class MiyabiSkinManager
    {
        static int MAX_SLOTS = 6; // 你写了多少个 Slot 就改成多少

        public static Dictionary<string, string> skinDatas = new Dictionary<string, string>();

        /// <summary>
        /// 皮肤预览图路径。Key 与 skinDatas 共用同一套键名。
        /// </summary>
        public static Dictionary<string, string> previewDatas = new Dictionary<string, string>();

        static MiyabiSkinManager()
        {
            // ---- 默认皮肤名称 ----
            skinDatas["MIYABISTS2-COMBAT_SELECTED_SLOT.Slot0"] = "烈露濯霜";
            skinDatas["MIYABISTS2-REST_SELECTED_SLOT.Slot0"] = "小雅";
            skinDatas["MIYABISTS2-SHOP_SELECTED_SLOT.Slot0"] = "烈露濯霜";

            // ---- 默认皮肤预览图 ----
            previewDatas["MIYABISTS2-COMBAT_SELECTED_SLOT.Slot0"] = "res://images/charui/miyabi_char_33.png";
            previewDatas["MIYABISTS2-REST_SELECTED_SLOT.Slot0"] = "res://images/charui/little_miyabi.png";
            previewDatas["MIYABISTS2-SHOP_SELECTED_SLOT.Slot0"] = "res://images/charui/miyabi_char_33.png";
        }

        public static void RegisterCombatSkin(string name, string tscnPath, string previewPath = null)
        {
            if (Miyabi.CombatSkinPaths.Count < MAX_SLOTS) // 取决于你写了多少个 Slot
            {
                Miyabi.CombatSkinPaths.Add(tscnPath);
                tscnPath?.RegisterSceneForConversion<NCreatureVisuals>();

                int slot = Miyabi.CombatSkinPaths.Count - 1;
                GD.Print($"[MiyabiSkinManager] 外部皮肤成功入驻槽位 Slot_{slot}!");
                skinDatas.Add($"MIYABISTS2-COMBAT_SELECTED_SLOT.Slot{slot}", name);

                if (!string.IsNullOrEmpty(previewPath))
                {
                    previewDatas[$"MIYABISTS2-COMBAT_SELECTED_SLOT.Slot{slot}"] = previewPath;
                    GD.Print($"[MiyabiSkinManager] 战斗皮肤预览图已注册: Slot{slot} -> {previewPath}");
                }
            }
        }

        public static void RegisterRestSkin(string name, string tscnPath, string previewPath = null)
        {
            if (Miyabi.RestSkinPaths.Count < MAX_SLOTS) // 取决于你写了多少个 Slot
            {
                Miyabi.RestSkinPaths.Add(tscnPath);
                tscnPath?.RegisterSceneForConversion<NRestSiteCharacter>();

                int slot = Miyabi.RestSkinPaths.Count - 1;
                GD.Print($"[MiyabiSkinManager] 外部皮肤成功入驻槽位 Slot_{slot}!");
                skinDatas.Add($"MIYABISTS2-REST_SELECTED_SLOT.Slot{slot}", name);

                if (!string.IsNullOrEmpty(previewPath))
                {
                    previewDatas[$"MIYABISTS2-REST_SELECTED_SLOT.Slot{slot}"] = previewPath;
                    GD.Print($"[MiyabiSkinManager] 休息皮肤预览图已注册: Slot{slot} -> {previewPath}");
                }
            }
        }

        public static void RegisterShopSkin(string name, string tscnPath, string previewPath = null)
        {
            if (Miyabi.ShopSkinPaths.Count < MAX_SLOTS) // 取决于你写了多少个 Slot
            {
                Miyabi.ShopSkinPaths.Add(tscnPath);
                tscnPath?.RegisterSceneForConversion<NMerchantCharacter>();

                int slot = Miyabi.ShopSkinPaths.Count - 1;
                GD.Print($"[MiyabiSkinManager] 外部皮肤成功入驻槽位 Slot_{slot}!");
                skinDatas.Add($"MIYABISTS2-SHOP_SELECTED_SLOT.Slot{slot}", name);

                if (!string.IsNullOrEmpty(previewPath))
                {
                    previewDatas[$"MIYABISTS2-SHOP_SELECTED_SLOT.Slot{slot}"] = previewPath;
                    GD.Print($"[MiyabiSkinManager] 商店皮肤预览图已注册: Slot{slot} -> {previewPath}");
                }
            }
        }

        public static void PreSkinRegister()
        {
            "res://scenes/miyabi_char.tscn".RegisterSceneForConversion<NCreatureVisuals>();
            "res://scenes/Miyabi_Rest.tscn".RegisterSceneForConversion<NRestSiteCharacter>();
            "res://scenes/Miyabi_Shop.tscn".RegisterSceneForConversion<NMerchantCharacter>();
        }

        public static void UpdateSkin(string key, int slot, string skinName)
        {
            try
            {
                // 1. 直接通过原版单例抓取设置界面的本地化表
                LocTable settingsUiTable = LocManager.Instance.GetTable("settings_ui");
                GD.PrintErr($"[MiyabiSkinManager] 主动更新本地化失败: 未找到 settings_ui 表");
                if (settingsUiTable == null) return;

                // 2. 构造包
                var updateDict = new Dictionary<string, string>
                {
                    { $"MIYABISTS2-{key}_SELECTED_SLOT.Slot{Miyabi.ShopSkinPaths.Count - 1}", skinName }
                };

                // 3. 反射调用 MergeWith
                var mergeMethod = typeof(LocTable).GetMethod("MergeWith", new Type[] { typeof(Dictionary<string, string>) });
                if (mergeMethod != null)
                {
                    mergeMethod.Invoke(settingsUiTable, new object[] { updateDict });
                    GD.Print($"[MiyabiSkinManager] 成功修改 Key: MIYABISTS2-{key}_SELECTED_SLOT.Slot{Miyabi.ShopSkinPaths.Count - 1} -> {skinName}");
                }
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"[MiyabiSkinManager] 主动更新本地化失败: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(LocManager))]
    public class LocManagerEarlyInjectPatch
    {
        // 🎯 目标方法：SetLanguageInternal（私有实例方法）
        [HarmonyPatch("SetLanguageInternal")]
        [HarmonyPostfix]
        public static void Postfix(string language, Dictionary<string, LocTable> tables)
        {
            // 1. 我们只在加载中文（或者你想处理的语言）时介入
            if (language != "zhs") return;

            GD.Print($"[MiyabiEarlyInject] 🎬 LocManager 已读取完本地本地化文件。当前语言: {language}");

            // 2. 抢在 STS2RitsuLib 之前，从刚读好的 tables 字典里把 "settings_ui" 表捞出来
            if (tables != null && tables.TryGetValue("settings_ui", out LocTable settingsUiTable))
            {
                try
                {
                    // 3. 准备好你想要抢占的 Key-Value 映射
                    // 哪怕此时外部皮肤还没入驻，你可以先把所有槽位的默认文本或者占位文本写进去
                    var earlyData = MiyabiSkinManager.skinDatas.Count > 0 ? MiyabiSkinManager.skinDatas : new Dictionary<string, string>();


                    // 4. 反射调用 MergeWith 强行写入
                    var mergeMethod = typeof(LocTable).GetMethod("MergeWith", new Type[] { typeof(Dictionary<string, string>) });
                    if (mergeMethod != null)
                    {
                        mergeMethod.Invoke(settingsUiTable, new object[] { earlyData });
                        GD.Print($"[MiyabiEarlyInject] 💥 成功抢占 settings_ui 表！提前注入了 {earlyData.Count} 个 Key。STS2RitsuLib 将无法覆盖它们。");
                    }
                    else
                    {
                        // 备份暴力反射方案
                        InjectViaReflection(settingsUiTable, earlyData);
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[MiyabiEarlyInject] 抢先注入本地化失败: {ex.Message}");
                }
            }
        }

        private static void InjectViaReflection(LocTable table, Dictionary<string, string> data)
        {
            var fields = typeof(LocTable).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Dictionary<string, string>))
                {
                    var dict = (Dictionary<string, string>)field.GetValue(table);
                    if (dict != null)
                    {
                        foreach (var kvp in data)
                        {
                            dict[kvp.Key] = kvp.Value;
                        }
                        GD.Print("[MiyabiEarlyInject] 暴力反射私有字典卡位成功！");
                        break;
                    }
                }
            }
        }
    }

    //[HarmonyPatch(typeof(LocTable))]
    //public class LocTableInjectPatch
    //{
    //    // 标记是否已经注入过，避免重复合并浪费性能
    //    private static bool _hasInjectedMiyabiKeys = false;

    //    [HarmonyPatch(nameof(LocTable.HasEntry), new Type[] { typeof(string) })]
    //    [HarmonyPrefix]
    //    public static void Prefix(LocTable __instance, string key)
    //    {
    //        // 🎯 1. 寻找配置表：如果当前 UI 索要的 Key 属于设置界面（比如带有了你的 mod 前缀或 settings 关键字）
    //        // 且我们还没有对当前这个 LocTable 实例进行过注入
    //        if (key.Contains("miyabi_") && !_hasInjectedMiyabiKeys)
    //        {
    //            try
    //            {
    //                // 🎯 2. 准备你要添加或修改的 Key-Value 数据
    //                var customData = MiyabiSkinManager.skinDatas; // 直接拿之前注册的皮肤数据字典，Key 是 MIYABISTS2-xxx.SlotX，Value 是对应的显示名称

    //                // 🎯 3. 寻找原版的 MergeWith 方法
    //                // 源码里是：public void MergeWith(Dictionary<string, string> other)
    //                var mergeMethod = typeof(LocTable).GetMethod("MergeWith", new Type[] { typeof(Dictionary<string, string>) });

    //                if (mergeMethod != null)
    //                {
    //                    // 执行合并，数据会直接注入到原版的内存字典中
    //                    mergeMethod.Invoke(__instance, new object[] { customData });

    //                    _hasInjectedMiyabiKeys = true; // 注入成功，锁定
    //                    GD.Print("[MiyabiSkinManager] 💥 成功在运行时向 LocTable 动态注入/修改了皮肤本地化数据！");
    //                }
    //                else
    //                {
    //                    GD.PrintErr("[MiyabiSkinManager] 未找到 MergeWith 方法，尝试使用反射兜底方案...");
    //                    // 兜底：如果没找到 MergeWith，直接反射私有字典塞进去
    //                    InjectViaReflection(__instance, customData);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                GD.PrintErr($"[MiyabiSkinManager] 注入本地化失败: {ex.Message}");
    //            }
    //        }
    //    }

    //    // 兜底反射方案：直接暴力往原版的 Dictionary 里面塞数据
    //    private static void InjectViaReflection(LocTable table, Dictionary<string, string> data)
    //    {
    //        var fields = typeof(LocTable).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
    //        foreach (var field in fields)
    //        {
    //            if (field.FieldType == typeof(Dictionary<string, string>))
    //            {
    //                var dict = (Dictionary<string, string>)field.GetValue(table);
    //                if (dict != null)
    //                {
    //                    foreach (var kvp in data)
    //                    {
    //                        dict[kvp.Key] = kvp.Value; // 存在则覆盖，不存在则添加
    //                    }
    //                    _hasInjectedMiyabiKeys = true;
    //                    GD.Print("[MiyabiSkinManager] 通过反射私有字典注入成功！");
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
}
