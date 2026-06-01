using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Relics.SpecRelic;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Char;

public class Miyabi : PlaceholderCharacterModel
{
    public const string CharacterId = "Miyabi_Sakura";

    public static readonly Color Color = new("4682B4");


    public MiyabiCombatSkinSlot CombatSkinSlot => MiyabiModConfig.CombatSelectedSlot;
    public MiyabiRestSkinSlot RestSkinSlot => MiyabiModConfig.RestSelectedSlot;
    public MiyabiShopSkinSlot ShopSkinSlot => MiyabiModConfig.ShopSelectedSlot;


    public static List<string> CombatSkinPaths = new()
    {
        "res://scenes/miyabi_char.tscn" // Slot 0 默认是原皮肤
    };
    public static List<string> RestSkinPaths = new()
    {
        "res://scenes/Miyabi_Rest.tscn" // Slot 0 默认是原皮肤
    };
    public static List<string> ShopSkinPaths = new()
    {
        "res://scenes/Miyabi_Shop.tscn" // Slot 0 默认是原皮肤
    };

    public string CombatDynamicVisualPath
    {
        get
        {
            int index = (int)CombatSkinSlot; // 把枚举转成 0, 1, 2, 3...

            // 安全防御：如果玩家在工具里选了 Slot_3，但实际上只下载了 1 个皮肤（List长度只有2）
            if (index < CombatSkinPaths.Count)
            {
                return CombatSkinPaths[index];
            }
            return CombatSkinPaths[0]; // 越界了就安全回退到默认皮肤
        }
    }

    public string RestDynamicVisualPath
        {
            get
            {
                int index = (int)RestSkinSlot; // 把枚举转成 0, 1, 2, 3...
    
                // 安全防御：如果玩家在工具里选了 Slot_3，但实际上只下载了 1 个皮肤（List长度只有2）
                if (index < RestSkinPaths.Count)
                {
                    return RestSkinPaths[index];
                }
                return RestSkinPaths[0]; // 越界了就安全回退到默认皮肤
            }
        }

    public string ShopDynamicVisualPath
        {
            get
            {
                int index = (int)ShopSkinSlot; // 把枚举转成 0, 1, 2, 3...
    
                // 安全防御：如果玩家在工具里选了 Slot_3，但实际上只下载了 1 个皮肤（List长度只有2）
                if (index < ShopSkinPaths.Count)
                {
                    GD.Print($"[MiyabiSkin]: Load Skin {index}");
                    return ShopSkinPaths[index];
                }
                return ShopSkinPaths[0]; // 越界了就安全回退到默认皮肤
            }
        }

    //public static void RegisterCombatSkin(string tscnPath)
    //{
    //    if (CombatSkinPaths.Count < 4) // 取决于你写了多少个 Slot
    //    {
    //        CombatSkinPaths.Add(tscnPath);
    //        GD.Print($"[MiyabiMod] 外部皮肤成功入驻槽位 Slot_{CombatSkinPaths.Count - 1}!");
    //    }
    //}

    //public static void RegisterRestSkin(string tscnPath)
    //    {
    //        if (RestSkinPaths.Count < 4) // 取决于你写了多少个 Slot
    //        {
    //            RestSkinPaths.Add(tscnPath);
    //            GD.Print($"[MiyabiMod] 外部皮肤成功入驻槽位 Slot_{RestSkinPaths.Count - 1}!");
    //        }
    //    }

    //public static void RegisterShopSkin(string tscnPath)
    //    {
    //        if (ShopSkinPaths.Count < 4) // 取决于你写了多少个 Slot
    //        {
    //            ShopSkinPaths.Add(tscnPath);
    //            GD.Print($"[MiyabiMod] 外部皮肤成功入驻槽位 Slot_{ShopSkinPaths.Count - 1}!");
    //        }
    //    }

    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => Color;//new(0.1f, 0.1f, 1f);

    public override Color MapDrawingColor => Color;

    public override string PlaceholderID => "defect";


    //public override Color NameColor => Color;
    public override Color NameColor => StsColors.blue;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 52;

    /// <summary>
    /// 初始卡组。你可以在这里添加需要卡组。
    /// </summary>

    private IEnumerable<CardModel> GetDefaultDeck() => [
        ModelDb.Card<FengHua>(),
        ModelDb.Card<FengHua>(),
        ModelDb.Card<FengHua>(),
        ModelDb.Card<FengHua>(),
        ModelDb.Card<FengHua>(),
        ModelDb.Card<ShuiNiao>(),
        ModelDb.Card<ShuiNiao>(),
        ModelDb.Card<MiyabiBlock>(),
        ModelDb.Card<MiyabiBlock>(),
        ModelDb.Card<ShenXue>()
    ];

    private IEnumerable<CardModel> GetBangbooDeck() => [
        ModelDb.Card<BangbooSummonOne>(),
        ModelDb.Card<BangbooSummonOne>(),
        ModelDb.Card<BangbooSummonOne>(),
        ModelDb.Card<BangbooSummonOne>(),
        ModelDb.Card<BangbooActiveOne>(),
        ModelDb.Card<BangbooActiveOne>(),
        ModelDb.Card<BangbooChargeAll>(),
        ModelDb.Card<BangbooHelpmeOne>(),
        ModelDb.Card<BangbooHelpmeOne>(),
        ModelDb.Card<BangbooUseOnemore>()
    ];


    public override IEnumerable<CardModel> StartingDeck
    {
        get
        {
            switch (MiyabiModConfig.FunPileSelected)
            {
                case MiyabiFunPile.AllBangboo:
                    return GetBangbooDeck(); // 🌟 此时调用，ModelDb 100% 已经就绪了！

                default:
                    return GetDefaultDeck();
            }
        }
    }

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SwordNotailRelic>(),
        ModelDb.Relic<SectionSixRelic>(),
        ModelDb.Relic<ChoukaRelic>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        MiyabiCombatService.ResetAnoT();
        MiyabiCombatService.ResetDazeT();
        MiyabiCombatService.ResetFrostT();

        MiyabiCombatService.ResetCheck();
    }

    public override CardPoolModel CardPool => ModelDb.CardPool<MiyabiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MiyabiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MiyabiPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://images/charui/icon.png";
    public override string CustomCharacterSelectIconPath => "res://images/charui/Miyabi_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/charui/char_select_char_name_locked.png";
    //public override string CustomMapMarkerPath => "res://images/charui/map_marker_char_name.png";
    // 人物模型tscn路径。要自定义见下。
    public override string CustomVisualPath => CombatDynamicVisualPath;
    // 卡牌拖尾路径。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像2号。
    public override string CustomIconPath => "res://scenes/miyabi_icon.tscn";
    // 能量表盘tscn路径。要自定义见下。
    //public override string CustomEnergyCounterPath => "res://scenes/miyabi_energy_counter.tscn";
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
    public override string CustomCharacterSelectBg => "res://scenes/char_select/char_select_bg_miyabi.tscn";
    // 人物选择图标。
    //public override string CustomCharacterSelectIconPath => "res://test/images/char_select_test.png";
    // 人物选择图标-锁定状态。
    //public override string CustomCharacterSelectLockedIconPath => "res://test/images/char_select_test_locked.png";
    // 人物选择过渡动画。
    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "res://images/charui/icon.png";
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。
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