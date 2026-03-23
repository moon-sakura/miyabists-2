using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Relics;

namespace Miyabists2.Scripts.Char;

public class Miyabi : PlaceholderCharacterModel
{
    public const string CharacterId = "Miyabi_Sakura";

    public static readonly Color Color = new("4682B4");

    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0.1f, 0.1f, 1f);

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 40;

    /// <summary>
    /// 初始卡组。你可以在这里添加需要卡组。
    /// </summary>
    public override IEnumerable<CardModel> StartingDeck => [
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

    /// <summary>
    /// 初始遗物。你可以在这里添加一个或多个遗物，或者留空。
    /// </summary>
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SwordNotailRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<MiyabiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MiyabiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MiyabiPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => "res://images/charui/miyabi_icon.png";
    public override string CustomCharacterSelectIconPath => "res://images/charui/Miyabi_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/charui/char_select_char_name_locked.png";
    public override string CustomMapMarkerPath => "res://images/charui/map_marker_char_name.png";
    // 人物模型tscn路径。要自定义见下。
    //public override string CustomVisualPath => "res://test/scenes/test_character.tscn";
    // 卡牌拖尾路径。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像路径。
    //public override string CustomIconTexturePath => "res://icon.svg";
    // 人物头像2号。
    // public override string CustomIconPath => "res://scenes/ui/character_icons/ironclad_icon.tscn";
    // 能量表盘tscn路径。要自定义见下。
    //public override string CustomEnergyCounterPath => "res://test/scenes/test_energy_counter.tscn";
    // 篝火休息动画。
    // public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/ironclad_rest_site.tscn";
    // 商店人物动画。
    // public override string CustomMerchantAnimPath => "res://scenes/merchant/characters/ironclad_merchant.tscn";
    // 多人模式-手指。
    // public override string CustomArmPointingTexturePath => null;
    // 多人模式剪刀石头布-石头。
    // public override string CustomArmRockTexturePath => null;
    // 多人模式剪刀石头布-布。
    // public override string CustomArmPaperTexturePath => null;
    // 多人模式剪刀石头布-剪刀。
    // public override string CustomArmScissorsTexturePath => null;

    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://scenes/char_select/char_select_bg_miyabi.tscn";
    // 人物选择图标。
    //public override string CustomCharacterSelectIconPath => "res://test/images/char_select_test.png";
    // 人物选择图标-锁定状态。
    //public override string CustomCharacterSelectLockedIconPath => "res://test/images/char_select_test_locked.png";
    // 人物选择过渡动画。
    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    // public override string CustomMapMarkerPath => null;
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

}