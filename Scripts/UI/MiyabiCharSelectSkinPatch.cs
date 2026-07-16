using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Char;
using MegaCrit.Sts2.Core.Saves;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves;

namespace Miyabists2.Scripts.UI;

[HarmonyPatch]
public static class MiyabiCharSelectSkinPatch
{
    [Flags]
    private enum CharType { None = 0, Miyabi = 1, Yixuan = 2 }

    private static MiyabiDifficultyPanel _difficultyPanel;
    private static MiyabiSkinPanel _skinPanel;
    private static MiyabiFunPilePanel _funPilePanel;
    private static CharType _currentCharType = CharType.None;
    private static SkinType _currentSkinType = SkinType.Combat;
    private static bool _diffHandlerWired;
    private static bool _skinHandlerWired;
    private static bool _funPileHandlerWired;

    // ============================================================
    // Patch
    // ============================================================

    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    public static void Postfix(
        NCharacterSelectScreen __instance,
        NCharacterSelectButton charSelectButton,
        CharacterModel characterModel)
    {
        CharType charType = characterModel is Miyabi ? CharType.Miyabi
            : characterModel is Yixuan ? CharType.Yixuan
            : CharType.None;

        bool showPanels = charType != CharType.None && MiyabiModConfig.MiyabiPanelOpen;

        string language = SaveManager.Instance != null
            ? MiyabiUILoc.NormalizeLang(SaveManager.Instance.SettingsSave.Language)
            : "eng";
        GD.Print($"[MiyabiSkinPatch] Postfix | char={characterModel?.Id.Entry ?? "null"} "
            + $"| charType={charType} | lang={language} | panelOpen={MiyabiModConfig.MiyabiPanelOpen} | show={showPanels}");

        try
        {
            if (showPanels)
            {
                _currentCharType = charType;

                // --- 进阶选项（共用） ---
                var diffPanel = GetOrCreateDifficultyPanel(__instance);
                diffPanel.Language = language;
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
                skinPanel.Language = language;
                if (!_skinHandlerWired)
                {
                    skinPanel.SkinChanged += OnSkinChanged;
                    skinPanel.SkinTypeChanged += OnSkinTypeChanged;
                    _skinHandlerWired = true;
                }
                LoadSkinData(skinPanel, _currentSkinType, charType, language);

                // --- 特殊挑战（按角色分开） ---
                var funPilePanel = GetOrCreateFunPilePanel(__instance);
                funPilePanel.Language = language;
                if (!_funPileHandlerWired)
                {
                    funPilePanel.FunPileChanged += OnFunPileChanged;
                    _funPileHandlerWired = true;
                }
                LoadFunPileData(funPilePanel, charType, language);
            }
            else
            {
                _currentCharType = CharType.None;
            }

            // 统一显隐
            bool diffValid = _difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel);
            bool skinValid = _skinPanel != null && GodotObject.IsInstanceValid(_skinPanel);
            bool funValid = _funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel);

            if (diffValid) _difficultyPanel.Visible = showPanels;
            if (skinValid) _skinPanel.Visible = showPanels;
            if (funValid) _funPilePanel.Visible = showPanels;

            GD.Print($"[MiyabiSkinPatch] 显隐 | show={showPanels} | diffV={diffValid} skinV={skinValid} funV={funValid}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[MiyabiSkinPatch] ❌ {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    // ============================================================
    // 面板创建
    // ============================================================

    private static MiyabiDifficultyPanel GetOrCreateDifficultyPanel(NCharacterSelectScreen screen)
    {
        if (_difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel))
            return _difficultyPanel;
        _difficultyPanel = new MiyabiDifficultyPanel { Name = "MiyabiDifficultyPanel", Position = new Vector2(1460, 40) };
        screen.AddChild(_difficultyPanel);
        _diffHandlerWired = false;
        return _difficultyPanel;
    }

    private static MiyabiSkinPanel GetOrCreateSkinPanel(NCharacterSelectScreen screen)
    {
        if (_skinPanel != null && GodotObject.IsInstanceValid(_skinPanel))
            return _skinPanel;
        _skinPanel = new MiyabiSkinPanel { Name = "MiyabiSkinPanel", Position = new Vector2(1460, 380) };
        screen.AddChild(_skinPanel);
        _skinHandlerWired = false;
        return _skinPanel;
    }

    private static MiyabiFunPilePanel GetOrCreateFunPilePanel(NCharacterSelectScreen screen)
    {
        if (_funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel))
            return _funPilePanel;
        _funPilePanel = new MiyabiFunPilePanel { Name = "MiyabiFunPilePanel", Position = new Vector2(1460, 770) };
        screen.AddChild(_funPilePanel);
        _funPileHandlerWired = false;
        return _funPilePanel;
    }

    // ============================================================
    // 事件：难度（共用）
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
        LoadSkinData(_skinPanel, newType, _currentCharType, _skinPanel.Language);
    }

    private static void OnSkinChanged(int newIndex)
    {
        if (_currentCharType == CharType.None) return;
        if (_skinPanel == null || !GodotObject.IsInstanceValid(_skinPanel)) return;

        switch (_currentSkinType)
        {
            case SkinType.Combat:
                if (_currentCharType == CharType.Miyabi)
                    MiyabiModConfig.CombatSelectedSlot = (MiyabiCombatSkinSlot)newIndex;
                else
                    MiyabiModConfig.YixuanCombatSelectedSlot = (YixuanCombatSkinSlot)newIndex;
                break;
            case SkinType.Rest:
                if (_currentCharType == CharType.Miyabi)
                    MiyabiModConfig.RestSelectedSlot = (MiyabiRestSkinSlot)newIndex;
                else
                    MiyabiModConfig.YixuanRestSelectedSlot = (YixuanRestSkinSlot)newIndex;
                break;
            case SkinType.Shop:
                if (_currentCharType == CharType.Miyabi)
                    MiyabiModConfig.ShopSelectedSlot = (MiyabiShopSkinSlot)newIndex;
                else
                    MiyabiModConfig.YixuanShopSelectedSlot = (YixuanShopSkinSlot)newIndex;
                break;
        }

        if (_currentSkinType == SkinType.Combat)
        {
            var screen = _skinPanel.GetParent();
            if (screen != null)
                RefreshBackground(screen, _currentCharType);
        }
    }

    // ============================================================
    // 事件：特殊挑战（共用）
    // ============================================================

    private static void OnFunPileChanged(int newIndex)
    {
        if (_currentCharType == CharType.Miyabi)
            MiyabiModConfig.MiyabiFunPileSelected = (MiyabiFunPile)newIndex;
        else if (_currentCharType == CharType.Yixuan)
            MiyabiModConfig.YixuanFunPileSelected = (YixuanFunPile)newIndex;
    }

    // ============================================================
    // 皮肤数据加载
    // ============================================================

    private static void LoadSkinData(MiyabiSkinPanel panel, SkinType skinType, CharType charType, string language)
    {
        int skinCount;
        string keyPrefix;
        int currentSlot;
        string previewFallbackPrefix;

        switch (charType)
        {
            case CharType.Miyabi:
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
                    default: return;
                }
                break;

            case CharType.Yixuan:
                switch (skinType)
                {
                    case SkinType.Combat:
                        skinCount = Yixuan.CombatSkinPaths.Count;
                        keyPrefix = "YIXUAN_COMBAT";
                        currentSlot = (int)MiyabiModConfig.YixuanCombatSelectedSlot;
                        previewFallbackPrefix = "yixuan_combat";
                        break;
                    case SkinType.Rest:
                        skinCount = Yixuan.RestSkinPaths.Count;
                        keyPrefix = "YIXUAN_REST";
                        currentSlot = (int)MiyabiModConfig.YixuanRestSelectedSlot;
                        previewFallbackPrefix = "yixuan_rest";
                        break;
                    case SkinType.Shop:
                        skinCount = Yixuan.ShopSkinPaths.Count;
                        keyPrefix = "YIXUAN_SHOP";
                        currentSlot = (int)MiyabiModConfig.YixuanShopSelectedSlot;
                        previewFallbackPrefix = "yixuan_shop";
                        break;
                    default: return;
                }
                break;

            default: return;
        }

        var names = new List<string>(skinCount);
        var previews = new List<string>(skinCount);

        for (int i = 0; i < skinCount; i++)
        {
            string dataKey = $"MIYABISTS2-{keyPrefix}_SELECTED_SLOT.Slot{i}";
            string name = MiyabiSkinManager.GetSkinName(dataKey, language);
            names.Add(name);

            if (MiyabiSkinManager.previewDatas.TryGetValue(dataKey, out var previewPath))
                previews.Add(previewPath);
            else
                previews.Add($"res://images/skins/{previewFallbackPrefix}_preview_{i}.png");
        }

        if (currentSlot >= skinCount) currentSlot = 0;

        panel.CurrentSkinType = skinType;
        panel.PreviewImagePaths = previews;
        panel.SkinDisplayNames = names;
        panel.CurrentIndex = currentSlot;
    }

    // ============================================================
    // 特殊挑战数据加载（按角色分开）
    // ============================================================

    private static void LoadFunPileData(MiyabiFunPilePanel panel, CharType charType, string language)
    {
        (string name, string desc)[] options;
        int currentIndex;

        switch (charType)
        {
            case CharType.Miyabi:
                options = MakeOptions(language,
                    "funp_miyabi_default",
                    "funp_miyabi_bangboo",
                    "funp_miyabi_bee",
                    "funp_miyabi_recorder",
                    "funp_miyabi_grace");
                currentIndex = (int)MiyabiModConfig.MiyabiFunPileSelected;
                break;

            case CharType.Yixuan:
                options = MakeOptions(language,
                    "funp_yixuan_default",
                    "funp_yixuan_bangboo",
                    "funp_yixuan_bee",
                    "funp_yixuan_recorder");
                currentIndex = (int)MiyabiModConfig.YixuanFunPileSelected;
                break;

            default: return;
        }

        panel.SetOptions(options, currentIndex);
    }

    private static (string name, string desc)[] MakeOptions(string language, params string[] locKeys)
    {
        var result = new (string name, string desc)[locKeys.Length];
        for (int i = 0; i < locKeys.Length; i++)
        {
            string fullText = MiyabiUILoc.Get(locKeys[i], language);
            int idx = fullText.IndexOf('：');
            string displayName = idx > 0 ? fullText.Substring(0, idx) : fullText;
            result[i] = (displayName, fullText);
        }
        return result;
    }

    // ============================================================
    // 背景刷新
    // ============================================================

    private static void RefreshBackground(Node screen, CharType charType)
    {
        var bgField = typeof(NCharacterSelectScreen).GetField("_bgContainer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var bgContainer = bgField?.GetValue(screen) as Control;
        if (bgContainer == null) return;

        foreach (Node child in bgContainer.GetChildren())
        {
            bgContainer.RemoveChild(child);
            child.QueueFree();
        }

        string bgPath = charType == CharType.Miyabi
            ? "res://scenes/char_select/char_select_bg_miyabi.tscn"
            : "res://scenes/_Yixuan/char_select_bg_yixuan.tscn";

        var scene = ResourceLoader.Load<PackedScene>(bgPath);
        if (scene != null)
        {
            var control = scene.Instantiate<Control>(PackedScene.GenEditState.Disabled);
            control.Name = (charType == CharType.Miyabi ? "Miyabi_Sakura" : "Yixuan") + "_bg";
            bgContainer.AddChild(control);
        }
    }
}
