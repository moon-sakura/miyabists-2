using BaseLib.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    public enum MiyabiCombatSkinSlot
    {
        Slot0 = 0,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5
    }
    public enum MiyabiRestSkinSlot
    {
        Slot0 = 0,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5
    }
    public enum MiyabiShopSkinSlot
    {
        Slot0 = 0,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5
    }

    public enum MiyabiSelectedHard
    {
        Zero = 0,
        Sb,
        Zn,
        Sn,
        Mn,
        Cd,
        Ni,
        Cu,
        Bi,
        Pb,
        Hg
    }

    public enum MiyabiFunPile
    {
        Default = 0,
        AllBangboo,
        BeeGroup,
        //UltimateGrace
    }

    [ConfigHoverTipsByDefault]
    public sealed class MiyabiModConfig : SimpleModConfig
    {
        [ConfigSection("CombatConfig")] // 创建一个战斗设置分组
        [ConfigHoverTip]
        public static MiyabiSelectedHard CombatHardSelected { get; set; } = MiyabiSelectedHard.Zero;

        //[ConfigHoverTip]
        public static bool MiyabiEnemiesStronger { get; set; } = false;

        //[ConfigSlider(0.5, 5.0, 0.1, Format = "{0:0.#}x")]
        //[ConfigHoverTip]
        //public static double MonsterHpMax { get; set; } = 1.0;

        //// 2. 限制造成伤害 (使用百分比滑块展示，范围 0% 到 100%)
        //[ConfigSlider(0.3, 1.5, 0.05, Format = "{0:0.#}x")]
        //[ConfigHoverTip]
        //public static double DamageDealtLimit { get; set; } = 1.0;

        //// 3. 受到更多伤害 (范围 1x 到 5x)
        //[ConfigSlider(0.5, 3.0, 0.1, Format = "{0:0.#}x")]
        //[ConfigHoverTip]
        //public static double DamageTakenMultiplier { get; set; } = 1.0;

        public static bool ChangeToAllPlayers { get; set; } = false;

        [ConfigSection("ElseConfig")] // 创建一个战斗设置分组

        public static bool MiyabiPanelOpen { get; set; } = true;

        [ConfigHoverTip]
        public static MiyabiFunPile FunPileSelected { get; set; } = MiyabiFunPile.Default;

        [ConfigHoverTip]
        public static MiyabiCombatSkinSlot CombatSelectedSlot { get; set; } = MiyabiCombatSkinSlot.Slot0;

        [ConfigHoverTip]
        public static MiyabiRestSkinSlot RestSelectedSlot { get; set; } = MiyabiRestSkinSlot.Slot0;

        [ConfigHoverTip]
        public static MiyabiShopSkinSlot ShopSelectedSlot { get; set; } = MiyabiShopSkinSlot.Slot0;

    }

    /// <summary>
    /// 把这个类重新请回来！
    /// 项目里所有卡牌、能力继续无脑调用 MiyabiModConfig.MiyabiEnemiesStronger
    /// 外部看起来全是普通的 static，但内部悄悄对接了新库的 Binding，一拍不改！
    /// </summary>
    //public static class MiyabiModConfig
    //{
    //    public static MiyabiSelectedHard CombatHardSelected
    //    {
    //        get => MiyabiModConfigPage.CombatHardBinding.Read();
    //        set => MiyabiModConfigPage.CombatHardBinding.Write(value);
    //    }

    //    public static bool MiyabiEnemiesStronger
    //    {
    //        get => MiyabiModConfigPage.EnemiesStrongerBinding.Read();
    //        set => MiyabiModConfigPage.EnemiesStrongerBinding.Write(value);
    //    }

    //    public static bool ChangeToAllPlayers
    //    {
    //        get => MiyabiModConfigPage.ChangeToAllPlayersBinding.Read();
    //        set => MiyabiModConfigPage.ChangeToAllPlayersBinding.Write(value);
    //    }

    //    public static bool MiyabiPanelOpen
    //    {
    //        get => MiyabiModConfigPage.PanelOpenBinding.Read();
    //        set => MiyabiModConfigPage.PanelOpenBinding.Write(value);
    //    }

    //    public static MiyabiFunPile FunPileSelected
    //    {
    //        get => MiyabiModConfigPage.FunPileBinding.Read();
    //        set => MiyabiModConfigPage.FunPileBinding.Write(value);
    //    }

    //    public static MiyabiCombatSkinSlot CombatSelectedSlot
    //    {
    //        get => MiyabiModConfigPage.CombatSkinBinding.Read();
    //        set => MiyabiModConfigPage.CombatSkinBinding.Write(value);
    //    }

    //    public static MiyabiRestSkinSlot RestSelectedSlot
    //    {
    //        get => MiyabiModConfigPage.RestSkinBinding.Read();
    //        set => MiyabiModConfigPage.RestSkinBinding.Write(value);
    //    }

    //    public static MiyabiShopSkinSlot ShopSelectedSlot
    //    {
    //        get => MiyabiModConfigPage.ShopSkinBinding.Read();
    //        set => MiyabiModConfigPage.ShopSkinBinding.Write(value);
    //    }
    //}

    ///// <summary>
    ///// 1. 纯粹的数据模型类（对应你原先的字段）
    ///// </summary>
    //public sealed class MiyabiSettingsData
    //{
    //    public MiyabiSelectedHard CombatHardSelected { get; set; } = MiyabiSelectedHard.Zero;
    //    public bool MiyabiEnemiesStronger { get; set; } = false;
    //    public bool ChangeToAllPlayers { get; set; } = false;
    //    public bool MiyabiPanelOpen { get; set; } = true;
    //    public MiyabiFunPile FunPileSelected { get; set; } = MiyabiFunPile.Default;
    //    public MiyabiCombatSkinSlot CombatSelectedSlot { get; set; } = MiyabiCombatSkinSlot.Slot0;
    //    public MiyabiRestSkinSlot RestSelectedSlot { get; set; } = MiyabiRestSkinSlot.Slot0;
    //    public MiyabiShopSkinSlot ShopSelectedSlot { get; set; } = MiyabiShopSkinSlot.Slot0;
    //}

    ///// <summary>
    ///// 2. 配置页面注册与绑定中心
    ///// </summary>
    //public static class MiyabiModConfigPage
    //{
    //    private const string DataKey = "MiyabiModSettings"; // 防撞Key

    //    #region 静态值绑定 (供项目内其他卡牌或UI随时调用读写)

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, MiyabiSelectedHard> CombatHardBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.CombatHardSelected,
    //        static (s, v) => s.CombatHardSelected = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, bool> EnemiesStrongerBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.MiyabiEnemiesStronger,
    //        static (s, v) => s.MiyabiEnemiesStronger = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, bool> ChangeToAllPlayersBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.ChangeToAllPlayers,
    //        static (s, v) => s.ChangeToAllPlayers = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, bool> PanelOpenBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.MiyabiPanelOpen,
    //        static (s, v) => s.MiyabiPanelOpen = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, MiyabiFunPile> FunPileBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.FunPileSelected,
    //        static (s, v) => s.FunPileSelected = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, MiyabiCombatSkinSlot> CombatSkinBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.CombatSelectedSlot,
    //        static (s, v) => s.CombatSelectedSlot = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, MiyabiRestSkinSlot> RestSkinBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.RestSelectedSlot,
    //        static (s, v) => s.RestSelectedSlot = v);

    //    public static readonly ModSettingsValueBinding<MiyabiSettingsData, MiyabiShopSkinSlot> ShopSkinBinding = new(
    //        Entry.ModId, DataKey, SaveScope.Profile,
    //        static s => s.ShopSelectedSlot,
    //        static (s, v) => s.ShopSelectedSlot = v);

    //    #endregion

    //    /// <summary>
    //    /// 在Mod初始化(Entry.Initialize)时调用此方法
    //    /// </summary>
    //    public static void Register()
    //    {
    //        // [步骤 A] 注册数据存储底层
    //        ModDataStore.For(Entry.ModId).Register<MiyabiSettingsData>(
    //            key: DataKey,
    //            fileName: "miyabi_settings.json",
    //            scope: SaveScope.Profile, // 随存档独立
    //            defaultFactory: () => new MiyabiSettingsData(),
    //            autoCreateIfMissing: true);

    //        // [步骤 B] 流式构建你的精致 UI 面板
    //        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
    //            .WithTitle(ModSettingsText.Literal("星见雅模组配置"))
    //            .WithModDisplayName(ModSettingsText.Literal("星见雅 (Miyabi)"))
    //            .WithVisibleOnHostSurfaces(ModSettingsHostSurface.MainMenu | ModSettingsHostSurface.RunPause)

    //            // === 战斗配置分组 ===
    //            .AddSection("CombatConfig", section => section
    //                .WithTitle(ModSettingsText.Literal("战斗配置"))

    //                // 1. 挑战难度下拉框
    //                .AddChoice("CombatHardSelected", ModSettingsText.Literal("挑战难度"), CombatHardBinding,
    //                    [
    //                        new(MiyabiSelectedHard.Zero, ModSettingsText.Literal("无进阶")),
    //                        // 如果有其他难度枚举，按照格式在下面累加：
    //                        // new(MiyabiSelectedHard.One, ModSettingsText.Literal("进阶 1")),
    //                    ], presentation: ModSettingsChoicePresentation.Dropdown)

    //                // 2. 加强模组敌人开关
    //                .AddToggle("MiyabiEnemiesStronger", ModSettingsText.Literal("加强模组敌人"), EnemiesStrongerBinding)

    //                // 3. 应用于所有角色开关
    //                .AddToggle("ChangeToAllPlayers", ModSettingsText.Literal("将战斗配置应用于所有角色"), ChangeToAllPlayersBinding)
    //            )

    //            // === 其它配置分组 ===
    //            .AddSection("ElseConfig", section => section
    //                .WithTitle(ModSettingsText.Literal("其它配置"))

    //                // 4. 面板开启开关
    //                .AddToggle("MiyabiPanelOpen", ModSettingsText.Literal("开启选人自定义面板"), PanelOpenBinding)

    //                // 5. 特殊初始卡组下拉框
    //                .AddChoice("FunPileSelected", ModSettingsText.Literal("特殊初始卡组"), FunPileBinding,
    //                    [
    //                        new(MiyabiFunPile.Default, ModSettingsText.Literal("默认卡组")),
    //                        new(MiyabiFunPile.AllBangboo, ModSettingsText.Literal("邦布卡组"))
    //                    ], presentation: ModSettingsChoicePresentation.Dropdown)

    //                // 6. 战斗皮肤下拉框
    //                .AddChoice("CombatSelectedSlot", ModSettingsText.Literal("星见雅战斗皮肤选项"), CombatSkinBinding,
    //                    [
    //                        new(MiyabiCombatSkinSlot.Slot0, ModSettingsText.Literal("银庭花信"))
    //                        // 如有其他皮肤样式：new(MiyabiCombatSkinSlot.Slot1, ModSettingsText.Literal("其它皮肤")),
    //                    ], presentation: ModSettingsChoicePresentation.Dropdown)

    //                // 7. 休息皮肤下拉框
    //                .AddChoice("RestSelectedSlot", ModSettingsText.Literal("星见雅休息皮肤选项"), RestSkinBinding,
    //                    [
    //                        new(MiyabiRestSkinSlot.Slot0, ModSettingsText.Literal("小雅"))
    //                    ], presentation: ModSettingsChoicePresentation.Dropdown)

    //                // 8. 商店皮肤下拉框
    //                .AddChoice("ShopSelectedSlot", ModSettingsText.Literal("星见雅商店皮肤选项"), ShopSkinBinding,
    //                    [
    //                        new(MiyabiShopSkinSlot.Slot0, ModSettingsText.Literal("烈露濯霜"))
    //                    ], presentation: ModSettingsChoicePresentation.Dropdown)
    //            ));
    //    }
    //}
}

