using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Service;
using System.Collections.Generic;

namespace Miyabists2.Scripts.UI;

/// <summary>
/// 在选人界面当 Miyabi 被选中时显示进阶选项 / 皮肤 / 特殊挑战三个面板。
/// 受 MiyabiModConfig.MiyabiPanelOpen 控制整体显隐。
/// </summary>
[HarmonyPatch]
public static class MiyabiCharSelectSkinPatch
{
    private static MiyabiDifficultyPanel _difficultyPanel;
    private static MiyabiSkinPanel _skinPanel;
    private static MiyabiFunPilePanel _funPilePanel;
    private static Miyabi _currentMiyabi;
    private static SkinType _currentSkinType = SkinType.Combat;
    private static bool _diffHandlerWired;
    private static bool _skinHandlerWired;
    private static bool _funPileHandlerWired;

    // ============================================================
    // Patch: NCharacterSelectScreen.SelectCharacter
    // ============================================================

    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    public static void Postfix(
        NCharacterSelectScreen __instance,
        NCharacterSelectButton charSelectButton,
        CharacterModel characterModel)
    {
        bool showPanels = characterModel is Miyabi && MiyabiModConfig.MiyabiPanelOpen;

        if (characterModel is Miyabi miyabi && MiyabiModConfig.MiyabiPanelOpen)
        {
            _currentMiyabi = miyabi;

            // --- 进阶选项 ---
            var diffPanel = GetOrCreateDifficultyPanel(__instance);
            if (!_diffHandlerWired)
            {
                diffPanel.LevelChanged += OnDifficultyChanged;
                diffPanel.EnemyStrongerChanged += OnEnemyStrongerChanged;
                _diffHandlerWired = true;
            }
            diffPanel.CurrentLevel = (int)MiyabiModConfig.CombatHardSelected;
            diffPanel.EnemyStronger = MiyabiModConfig.MiyabiEnemiesStronger;

            // --- 皮肤面板 ---
            var skinPanel = GetOrCreateSkinPanel(__instance);
            if (!_skinHandlerWired)
            {
                skinPanel.SkinChanged += OnSkinChanged;
                skinPanel.SkinTypeChanged += OnSkinTypeChanged;
                _skinHandlerWired = true;
            }
            LoadSkinData(skinPanel, _currentSkinType);

            // --- 特殊挑战 ---
            var funPilePanel = GetOrCreateFunPilePanel(__instance);
            if (!_funPileHandlerWired)
            {
                funPilePanel.FunPileChanged += OnFunPileChanged;
                _funPileHandlerWired = true;
            }
            funPilePanel.CurrentIndex = (int)MiyabiModConfig.FunPileSelected;
        }
        else
        {
            _currentMiyabi = null;
        }

        // 统一控制显隐
        bool diffValid = _difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel);
        bool skinValid = _skinPanel != null && GodotObject.IsInstanceValid(_skinPanel);
        bool funValid = _funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel);

        if (diffValid) _difficultyPanel.Visible = showPanels;
        if (skinValid) _skinPanel.Visible = showPanels;
        if (funValid) _funPilePanel.Visible = showPanels;
    }

    // ============================================================
    // 面板创建
    // ============================================================

    private static MiyabiDifficultyPanel GetOrCreateDifficultyPanel(NCharacterSelectScreen screen)
    {
        if (_difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel))
            return _difficultyPanel;

        _difficultyPanel = new MiyabiDifficultyPanel();
        _difficultyPanel.Name = "MiyabiDifficultyPanel";
        _difficultyPanel.Position = new Vector2(1460, 40);
        screen.AddChild(_difficultyPanel);
        _diffHandlerWired = false;
        return _difficultyPanel;
    }

    private static MiyabiSkinPanel GetOrCreateSkinPanel(NCharacterSelectScreen screen)
    {
        if (_skinPanel != null && GodotObject.IsInstanceValid(_skinPanel))
            return _skinPanel;

        _skinPanel = new MiyabiSkinPanel();
        _skinPanel.Name = "MiyabiSkinPanel";
        _skinPanel.Position = new Vector2(1460, 380);
        screen.AddChild(_skinPanel);
        _skinHandlerWired = false;
        return _skinPanel;
    }

    private static MiyabiFunPilePanel GetOrCreateFunPilePanel(NCharacterSelectScreen screen)
    {
        if (_funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel))
            return _funPilePanel;

        _funPilePanel = new MiyabiFunPilePanel();
        _funPilePanel.Name = "MiyabiFunPilePanel";
        _funPilePanel.Position = new Vector2(1460, 770);
        screen.AddChild(_funPilePanel);
        _funPileHandlerWired = false;
        return _funPilePanel;
    }

    // ============================================================
    // 事件：难度
    // ============================================================

    private static void OnDifficultyChanged(int newLevel)
    {
        MiyabiModConfig.CombatHardSelected = (MiyabiSelectedHard)newLevel;
    }

    private static void OnEnemyStrongerChanged(bool toggled)
    {
        MiyabiModConfig.MiyabiEnemiesStronger = toggled;
    }

    // ============================================================
    // 事件：皮肤
    // ============================================================

    private static void OnSkinTypeChanged(SkinType newType)
    {
        _currentSkinType = newType;
        if (_skinPanel == null || !GodotObject.IsInstanceValid(_skinPanel)) return;
        LoadSkinData(_skinPanel, newType);
    }

    private static void OnSkinChanged(int newIndex)
    {
        if (_currentMiyabi == null) return;
        if (_skinPanel == null || !GodotObject.IsInstanceValid(_skinPanel)) return;

        switch (_currentSkinType)
        {
            case SkinType.Combat:
                MiyabiModConfig.CombatSelectedSlot = (MiyabiCombatSkinSlot)newIndex;
                break;
            case SkinType.Rest:
                MiyabiModConfig.RestSelectedSlot = (MiyabiRestSkinSlot)newIndex;
                break;
            case SkinType.Shop:
                MiyabiModConfig.ShopSelectedSlot = (MiyabiShopSkinSlot)newIndex;
                break;
        }

        if (_currentSkinType == SkinType.Combat)
        {
            var screen = _skinPanel.GetParent();
            if (screen != null)
                RefreshBackground(screen, _currentMiyabi);
        }
    }

    // ============================================================
    // 事件：特殊挑战
    // ============================================================

    private static void OnFunPileChanged(int newIndex)
    {
        MiyabiModConfig.FunPileSelected = (MiyabiFunPile)newIndex;
    }

    // ============================================================
    // 皮肤数据加载
    // ============================================================

    private static void LoadSkinData(MiyabiSkinPanel panel, SkinType skinType)
    {
        int skinCount;
        string keyPrefix;
        int currentSlot;
        string previewFallbackPrefix;

        switch (skinType)
        {
            case SkinType.Combat:
                skinCount = Miyabi.CombatSkinPaths.Count;
                keyPrefix = "COMBAT";
                currentSlot = (int)MiyabiModConfig.CombatSelectedSlot;
                previewFallbackPrefix = "combat";
                break;
            case SkinType.Rest:
                skinCount = Miyabi.RestSkinPaths.Count;
                keyPrefix = "REST";
                currentSlot = (int)MiyabiModConfig.RestSelectedSlot;
                previewFallbackPrefix = "rest";
                break;
            case SkinType.Shop:
                skinCount = Miyabi.ShopSkinPaths.Count;
                keyPrefix = "SHOP";
                currentSlot = (int)MiyabiModConfig.ShopSelectedSlot;
                previewFallbackPrefix = "shop";
                break;
            default:
                return;
        }

        var names = new List<string>(skinCount);
        var previews = new List<string>(skinCount);

        for (int i = 0; i < skinCount; i++)
        {
            string dataKey = $"MIYABISTS2-{keyPrefix}_SELECTED_SLOT.Slot{i}";
            string name = MiyabiSkinManager.skinDatas.TryGetValue(dataKey, out var n) ? n : $"{skinType} Skin {i}";
            names.Add(name);

            if (MiyabiSkinManager.previewDatas.TryGetValue(dataKey, out var previewPath))
            {
                previews.Add(previewPath);
            }
            else
            {
                previews.Add($"res://images/skins/{previewFallbackPrefix}_preview_{i}.png");
            }
        }

        if (currentSlot >= skinCount) currentSlot = 0;

        panel.CurrentSkinType = skinType;
        panel.PreviewImagePaths = previews;
        panel.SkinDisplayNames = names;
        panel.CurrentIndex = currentSlot;
    }

    // ============================================================
    // 背景刷新
    // ============================================================

    private static void RefreshBackground(Node screen, Miyabi miyabi)
    {
        var bgField = typeof(NCharacterSelectScreen).GetField(
            "_bgContainer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var bgContainer = bgField?.GetValue(screen) as Control;
        if (bgContainer == null) return;

        foreach (Node child in bgContainer.GetChildren())
        {
            bgContainer.RemoveChild(child);
            child.QueueFree();
        }

        string bgPath = miyabi.CustomCharacterSelectBg;
        var scene = ResourceLoader.Load<PackedScene>(bgPath);
        if (scene != null)
        {
            var control = scene.Instantiate<Control>(PackedScene.GenEditState.Disabled);
            control.Name = miyabi.Id.Entry + "_bg";
            bgContainer.AddChild(control);
        }
    }
}