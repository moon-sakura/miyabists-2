global using BaseLib.Extensions;
global using STS2RitsuLib;
global using STS2RitsuLib.Interop.AutoRegistration;
global using STS2RitsuLib.Keywords;
global using STS2RitsuLib.Scaffolding.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts._Guang.Relics;
using Miyabists2.Scripts._Yixuan.Relics;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;
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

    public override string? PlaceholderCharacterId => "ironclad";

    // TODO: 添加皮肤管理（参考Miyabi的皮肤系统）
    public GuangCombatSkinSlot CombatSkinSlot => MiyabiModConfig.GuangCombatSelectedSlot;
    public GuangRestSkinSlot RestSkinSlot => MiyabiModConfig.GuangRestSelectedSlot;
    public GuangShopSkinSlot ShopSkinSlot => MiyabiModConfig.GuangShopSelectedSlot;

    // TODO: 添加皮肤路径列表
    public static List<string> CombatSkinPaths = new()
     {
         "res://scenes/_Guang/guang_char.tscn" // slot 0 默认是原皮肤
     };
    public static List<string> RestSkinPaths = new()
     {
         "res://scenes/_Guang/guang_Rest.tscn" // slot 0 默认是原皮肤
     };
    public static List<string> ShopSkinPaths = new()
     {
         "res://scenes/_Guang/guang_Shop.tscn" // slot 0 默认是原皮肤
     };

    public string CombatDynamicVisualPath
    {
        get
        {
            int index = (int)CombatSkinSlot;
            if (index < CombatSkinPaths.Count)
                return CombatSkinPaths[index];
            return CombatSkinPaths[0];
        }
    }

    public string RestDynamicVisualPath
    {
        get
        {
            int index = (int)RestSkinSlot;
            if (index < RestSkinPaths.Count)
                return RestSkinPaths[index];
            return RestSkinPaths[0];
        }
    }

    public string ShopDynamicVisualPath
    {
        get
        {
            int index = (int)ShopSkinSlot;
            if (index < ShopSkinPaths.Count)
            {
                GD.Print($"[GuangSkin]: Load Skin {index}");
                return ShopSkinPaths[index];
            }
            return ShopSkinPaths[0];
        }
    }

    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    private IEnumerable<StartingDeckEntry> GetDefaultDeck() => [
        new(typeof(KuaiJian), 4),
        new(typeof(GeDang), 3),
        new(typeof(YinCangLan), 1),
        new(typeof(BuLiuXing), 1),
        new(typeof(ZhaoYing), 1),
    ];

    private IEnumerable<StartingDeckEntry> GetBangbooDeck() => [
        new(typeof(BangbooSummonOne), 4),
        new(typeof(EnnDanmu), 1),
        new(typeof(YoushiHuoliXueshuo), 1),
        new(typeof(BangbooActiveOne), 1),
        new(typeof(BangbooChargeAll), 1),
        new(typeof(BangbooHelpmeOne), 1),
        new(typeof(BangbooUseOnemore), 1),
    ];

    /// <summary>
    /// 初始卡组。你可以在这里添加需要卡组。
    /// TODO: 替换为YeshunGuang专属卡牌
    /// </summary>
    protected override IEnumerable<StartingDeckEntry> StartingDeckEntries
    {
        get
        {
            switch (MiyabiModConfig.GuangFunPileSelected)
            {
                case GuangFunPile.Default:
                    return GetDefaultDeck();
                case GuangFunPile.AllBangboo:
                    return GetBangbooDeck();
                default:
                    return GetDefaultDeck();
            }
        }
    }

    // 初始遗物
    protected override IEnumerable<Type> StartingRelicTypes => [
        typeof(QingmingJianxiaRelic),
        typeof(YunkuiShanRelic),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        // TODO: 添加YeshunGuang专属房间进入逻辑
        // YeshunGuangCombatService.ResetAll();
    }

    // 人物头像路径。
    // TODO: 替换为YeshunGuang专属资源路径
    public override string CustomIconTexturePath => "res://images/_Guang/char/guangicon.png";
    public override string CustomCharacterSelectIconPath => "res://images/_Guang/char/guang_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/charui/char_select_char_name_locked.png";
    // 人物模型tscn路径。
    public override string CustomVisualsPath => CombatDynamicVisualPath;
    // 卡牌拖尾路径。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像2号。
    public override string CustomIconPath => "res://scenes/_Guang/guang_icon.tscn";
    // 能量表盘tscn路径。
    // public override string CustomEnergyCounterPath => "res://scenes/yeshunguang_energy_counter.tscn";
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
    // TODO: 替换为YeshunGuang专属背景
    public override string CustomCharacterSelectBgPath => "res://scenes/_Guang/char_select_bg_guang.tscn";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "res://images/_Guang/char/guangicon.png";
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
