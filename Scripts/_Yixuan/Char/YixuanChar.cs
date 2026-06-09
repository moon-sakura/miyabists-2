global using BaseLib.Extensions;
global using Godot;
global using Miyabists2.Scripts._Yixuan.Cards;
global using STS2RitsuLib;
global using STS2RitsuLib.Interop.AutoRegistration;
global using STS2RitsuLib.Keywords;
global using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts._Yixuan.Relics;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace Miyabists2.Scripts.Char;

[RegisterCharacter]
class Yixuan : ModCharacterTemplate<YixuanCardPool, YixuanRelicPool, YixuanPotionPool>
{
    public const string CharacterId = "Yixuan";

    // 墨金色 - 暗雅金色主题
    public static readonly Color Color = new("8B7539");

    // 角色名称颜色
    public override Color NameColor => Color;
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => Color;
    // 地图绘制颜色
    public override Color MapDrawingColor => Color;

    public override string? PlaceholderCharacterId => "regent";

    // TODO: 添加皮肤管理（参考Miyabi的皮肤系统）
    // public YixuanCombatSkinSlot CombatSkinSlot => YixuanModConfig.CombatSelectedSlot;
    // public YixuanRestSkinSlot RestSkinSlot => YixuanModConfig.RestSelectedSlot;
    // public YixuanShopSkinSlot ShopSkinSlot => YixuanModConfig.ShopSelectedSlot;

    // TODO: 添加皮肤路径列表
    public static List<string> CombatSkinPaths = new()
     {
         "res://scenes/_Yixuan/yixuan_char.tscn" // Slot 0 默认是原皮肤
     };
    public static List<string> RestSkinPaths = new()
     {
         "res://scenes/_Yixuan/yixuan_Rest.tscn" // Slot 0 默认是原皮肤
     };
    public static List<string> ShopSkinPaths = new()
     {
         "res://scenes/_Yixuan/yixuan_Shop.tscn" // Slot 0 默认是原皮肤
     };

    public string CombatDynamicVisualPath
    {
        get
        {
            //int index = (int)CombatSkinSlot;
            //if (index < CombatSkinPaths.Count)
            //    return CombatSkinPaths[index];
            return CombatSkinPaths[0];
        }
    }

    public string RestDynamicVisualPath
    {
        get
        {
            //int index = (int)RestSkinSlot;
            //if (index < RestSkinPaths.Count)
            //    return RestSkinPaths[index];
            return RestSkinPaths[0];
        }
    }

    public string ShopDynamicVisualPath
    {
        get
        {
            //int index = (int)ShopSkinSlot;
            //if (index < ShopSkinPaths.Count)
            //{
            //    GD.Print($"[YixuanSkin]: Load Skin {index}");
            //    return ShopSkinPaths[index];
            //}
            return ShopSkinPaths[0];
        }
    }

    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 88;
    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    /// <summary>
    /// 初始卡组。你可以在这里添加需要卡组。
    /// TODO: 替换为Yixuan专属卡牌
    /// </summary>
    protected override IEnumerable<StartingDeckEntry> StartingDeckEntries => [
        new(typeof(YixuanBlock), 3),
        new(typeof(XiaoyunJin), 4),
        // new(typeof(YixuanBlock), 2),
        // new(typeof(YixuanSpecial), 1),
    ];

    // 初始遗物
    protected override IEnumerable<Type> StartingRelicTypes => [
        typeof(QingmingShufaRelic),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        // TODO: 添加Yixuan专属房间进入逻辑
        // YixuanCombatService.ResetAll();
    }

    // 人物头像路径。
    // TODO: 替换为Yixuan专属资源路径
    public override string CustomIconTexturePath => "res://images/_YiXuan/char/yixuanicon.png";
    public override string CustomCharacterSelectIconPath => "res://images/_YiXuan/char/yixuan_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/charui/char_select_char_name_locked.png";
    // 人物模型tscn路径。
    public override string CustomVisualsPath => CombatDynamicVisualPath;
    // 卡牌拖尾路径。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像2号。
    public override string CustomIconPath => "res://scenes/_Yixuan/yixuan_icon.tscn";
    // 能量表盘tscn路径。
    // public override string CustomEnergyCounterPath => "res://scenes/yixuan_energy_counter.tscn";
    // 篝火休息动画。
    public override string CustomRestSiteAnimPath => RestDynamicVisualPath;
    // 商店人物动画。
    public override string CustomMerchantAnimPath => ShopDynamicVisualPath;
    // 多人模式-手指。
    public override string CustomArmPointingTexturePath => null;
    // 多人模式剪刀石头布-石头。
    public override string CustomArmRockTexturePath => null;
    // 多人模式剪刀石头布-布。
    public override string CustomArmPaperTexturePath => null;
    // 多人模式剪刀石头布-剪刀。
    public override string CustomArmScissorsTexturePath => null;

    // 人物选择背景。
    // TODO: 替换为Yixuan专属背景
    public override string CustomCharacterSelectBgPath => "res://scenes/_Yixuan/char_select_bg_yixuan.tscn";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "res://images/_YiXuan/char/yixuanicon.png";
    // 过渡音效。
    public override string CharacterTransitionSfx => "res://scenes/audio/select_ZhunbeiBadao.mp3";

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}