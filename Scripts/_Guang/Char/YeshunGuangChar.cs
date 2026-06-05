global using STS2RitsuLib.Interop.AutoRegistration;
global using STS2RitsuLib.Scaffolding.Content;
global using STS2RitsuLib.Keywords;
global using STS2RitsuLib;
global using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts._Guang.Relics;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace Miyabists2.Scripts.Char;

[RegisterCharacter]
class YeshunGuang : ModCharacterTemplate<YeshunGuangCardPool, YeshunGuangRelicPool, YeshunGuangPotionPool>
{
    public const string CharacterId = "YeshunGuang";

    // 白粉色 - 淡雅粉白主题
    public static readonly Color Color = new("E8B4C8");

    // 角色名称颜色
    public override Color NameColor => Color;
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => Color;
    // 地图绘制颜色
    public override Color MapDrawingColor => Color;

    public override string? PlaceholderCharacterId => "defect";

    // TODO: 添加皮肤管理（参考Miyabi的皮肤系统）
    // public YeshunGuangCombatSkinSlot CombatSkinSlot => YeshunGuangModConfig.CombatSelectedSlot;
    // public YeshunGuangRestSkinSlot RestSkinSlot => YeshunGuangModConfig.RestSelectedSlot;
    // public YeshunGuangShopSkinSlot ShopSkinSlot => YeshunGuangModConfig.ShopSelectedSlot;

    // TODO: 添加皮肤路径列表
    // public static List<string> CombatSkinPaths = new()
    // {
    //     "res://scenes/yeshunguang_char.tscn" // Slot 0 默认是原皮肤
    // };
    // public static List<string> RestSkinPaths = new()
    // {
    //     "res://scenes/YeshunGuang_Rest.tscn" // Slot 0 默认是原皮肤
    // };
    // public static List<string> ShopSkinPaths = new()
    // {
    //     "res://scenes/YeshunGuang_Shop.tscn" // Slot 0 默认是原皮肤
    // };

    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 52;
    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    /// <summary>
    /// 初始卡组。你可以在这里添加需要卡组。
    /// TODO: 替换为YeshunGuang专属卡牌
    /// </summary>
    protected override IEnumerable<StartingDeckEntry> StartingDeckEntries => [
        // TODO: 添加YeshunGuang初始卡牌
        // new(typeof(YeshunGuangAttack), 5),
        // new(typeof(YeshunGuangSkill), 2),
        // new(typeof(YeshunGuangBlock), 2),
        // new(typeof(YeshunGuangSpecial), 1),
    ];

    // 初始遗物
    protected override IEnumerable<Type> StartingRelicTypes => [
        typeof(QingmingJianxiaRelic),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        // TODO: 添加YeshunGuang专属房间进入逻辑
        // YeshunGuangCombatService.ResetAll();
    }

    // 人物头像路径。
    // TODO: 替换为YeshunGuang专属资源路径
    public override string CustomIconTexturePath => "res://images/charui/icon.png";
    public override string CustomCharacterSelectIconPath => "res://images/charui/Miyabi_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/charui/char_select_char_name_locked.png";
    // 人物模型tscn路径。
    // public override string CustomVisualsPath => CombatDynamicVisualPath;
    // 卡牌拖尾路径。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像2号。
    // public override string CustomIconPath => "res://scenes/yeshunguang_icon.tscn";
    // 能量表盘tscn路径。
    // public override string CustomEnergyCounterPath => "res://scenes/yeshunguang_energy_counter.tscn";
    // 篝火休息动画。
    // public override string CustomRestSiteAnimPath => RestDynamicVisualPath;
    // 商店人物动画。
    // public override string CustomMerchantAnimPath => ShopDynamicVisualPath;
    // 多人模式-手指。
    public override string CustomArmPointingTexturePath => null;
    // 多人模式剪刀石头布-石头。
    public override string CustomArmRockTexturePath => null;
    // 多人模式剪刀石头布-布。
    public override string CustomArmPaperTexturePath => null;
    // 多人模式剪刀石头布-剪刀。
    public override string CustomArmScissorsTexturePath => null;

    // 人物选择背景。
    // TODO: 替换为YeshunGuang专属背景
    public override string CustomCharacterSelectBgPath => "res://scenes/char_select/char_select_bg_miyabi.tscn";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "res://images/charui/icon.png";
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
