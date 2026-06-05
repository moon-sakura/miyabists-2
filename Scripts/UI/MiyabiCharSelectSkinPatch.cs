using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;

namespace Miyabists2.Scripts.UI;

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
        GD.Print($"[MiyabiSkinPatch] Postfix 触发 | char={characterModel?.Id.Entry ?? "null"} | "
            + $"isMiyabi={characterModel is Miyabi} | panelOpen={MiyabiModConfig.MiyabiPanelOpen}");

        bool showPanels = characterModel is Miyabi && MiyabiModConfig.MiyabiPanelOpen;
        GD.Print($"[MiyabiSkinPatch] showPanels={showPanels}");

        try
        {
            if (characterModel is Miyabi miyabi && MiyabiModConfig.MiyabiPanelOpen)
            {
                _currentMiyabi = miyabi;
                GD.Print("[MiyabiSkinPatch] Miyabi 已选中，开始初始化面板...");

                // --- 进阶选项 ---
                GD.Print("[MiyabiSkinPatch] 创建/获取进阶选项面板...");
                var diffPanel = GetOrCreateDifficultyPanel(__instance);
                GD.Print($"[MiyabiSkinPatch] 进阶面板 ready | valid={GodotObject.IsInstanceValid(diffPanel)}");
                if (!_diffHandlerWired)
                {
                    diffPanel.LevelChanged += OnDifficultyChanged;
                    diffPanel.EnemyStrongerChanged += OnEnemyStrongerChanged;
                    _diffHandlerWired = true;
                    GD.Print("[MiyabiSkinPatch] 进阶面板事件已绑定");
                }
                diffPanel.CurrentLevel = (int)MiyabiModConfig.CombatHardSelected;
                diffPanel.EnemyStronger = MiyabiModConfig.MiyabiEnemiesStronger;
                GD.Print($"[MiyabiSkinPatch] 进阶面板数据已设置 | level={diffPanel.CurrentLevel} | stronger={diffPanel.EnemyStronger}");

                // --- 皮肤面板 ---
                GD.Print("[MiyabiSkinPatch] 创建/获取皮肤面板...");
                var skinPanel = GetOrCreateSkinPanel(__instance);
                GD.Print($"[MiyabiSkinPatch] 皮肤面板 ready | valid={GodotObject.IsInstanceValid(skinPanel)}");
                if (!_skinHandlerWired)
                {
                    skinPanel.SkinChanged += OnSkinChanged;
                    skinPanel.SkinTypeChanged += OnSkinTypeChanged;
                    _skinHandlerWired = true;
                    GD.Print("[MiyabiSkinPatch] 皮肤面板事件已绑定");
                }
                LoadSkinData(skinPanel, _currentSkinType);
                GD.Print($"[MiyabiSkinPatch] 皮肤数据已加载 | type={_currentSkinType} | index={skinPanel.CurrentIndex}");

                // --- 特殊挑战 ---
                GD.Print("[MiyabiSkinPatch] 创建/获取特殊挑战面板...");
                var funPilePanel = GetOrCreateFunPilePanel(__instance);
                GD.Print($"[MiyabiSkinPatch] 特殊挑战面板 ready | valid={GodotObject.IsInstanceValid(funPilePanel)}");
                if (!_funPileHandlerWired)
                {
                    funPilePanel.FunPileChanged += OnFunPileChanged;
                    _funPileHandlerWired = true;
                    GD.Print("[MiyabiSkinPatch] 特殊挑战面板事件已绑定");
                }
                funPilePanel.CurrentIndex = (int)MiyabiModConfig.FunPileSelected;
                GD.Print($"[MiyabiSkinPatch] 特殊挑战数据已设置 | index={funPilePanel.CurrentIndex}");
            }
            else
            {
                _currentMiyabi = null;
            }

            // 统一控制显隐
            bool diffValid = _difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel);
            bool skinValid = _skinPanel != null && GodotObject.IsInstanceValid(_skinPanel);
            bool funValid = _funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel);
            GD.Print($"[MiyabiSkinPatch] 显隐控制 | show={showPanels} | diffValid={diffValid} | skinValid={skinValid} | funValid={funValid}");

            if (diffValid)
            {
                _difficultyPanel.Visible = showPanels;
                GD.Print($"[MiyabiSkinPatch] 进阶面板 Visible={_difficultyPanel.Visible}");
            }
            else if (showPanels)
            {
                GD.Print("[MiyabiSkinPatch] ⚠ 进阶面板无效但应该显示！");
            }

            if (skinValid)
            {
                _skinPanel.Visible = showPanels;
                GD.Print($"[MiyabiSkinPatch] 皮肤面板 Visible={_skinPanel.Visible}");
            }
            else if (showPanels)
            {
                GD.Print("[MiyabiSkinPatch] ⚠ 皮肤面板无效但应该显示！");
            }

            if (funValid)
            {
                _funPilePanel.Visible = showPanels;
                GD.Print($"[MiyabiSkinPatch] 特殊挑战面板 Visible={_funPilePanel.Visible}");
            }
            else if (showPanels)
            {
                GD.Print("[MiyabiSkinPatch] ⚠ 特殊挑战面板无效但应该显示！");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[MiyabiSkinPatch] ❌ 异常: {ex.GetType().Name} — {ex.Message}\n{ex.StackTrace}");
        }
    }

    // ============================================================
    // 面板创建
    // ============================================================

    private static MiyabiDifficultyPanel GetOrCreateDifficultyPanel(NCharacterSelectScreen screen)
    {
        if (_difficultyPanel != null && GodotObject.IsInstanceValid(_difficultyPanel))
        {
            GD.Print("[MiyabiSkinPatch] 进阶面板已存在，复用");
            return _difficultyPanel;
        }

        GD.Print("[MiyabiSkinPatch] 🔨 新建进阶面板...");
        _difficultyPanel = new MiyabiDifficultyPanel();
        _difficultyPanel.Name = "MiyabiDifficultyPanel";
        _difficultyPanel.Position = new Vector2(1460, 40);
        screen.AddChild(_difficultyPanel);
        _diffHandlerWired = false;
        GD.Print($"[MiyabiSkinPatch] 进阶面板已添加到屏幕 | parent={_difficultyPanel.GetParent()?.GetType().Name ?? "null"}");
        return _difficultyPanel;
    }

    private static MiyabiSkinPanel GetOrCreateSkinPanel(NCharacterSelectScreen screen)
    {
        if (_skinPanel != null && GodotObject.IsInstanceValid(_skinPanel))
        {
            GD.Print("[MiyabiSkinPatch] 皮肤面板已存在，复用");
            return _skinPanel;
        }

        GD.Print("[MiyabiSkinPatch] 🔨 新建皮肤面板...");
        _skinPanel = new MiyabiSkinPanel();
        _skinPanel.Name = "MiyabiSkinPanel";
        _skinPanel.Position = new Vector2(1460, 380);
        screen.AddChild(_skinPanel);
        _skinHandlerWired = false;
        GD.Print($"[MiyabiSkinPatch] 皮肤面板已添加到屏幕 | parent={_skinPanel.GetParent()?.GetType().Name ?? "null"}");
        return _skinPanel;
    }

    private static MiyabiFunPilePanel GetOrCreateFunPilePanel(NCharacterSelectScreen screen)
    {
        if (_funPilePanel != null && GodotObject.IsInstanceValid(_funPilePanel))
        {
            GD.Print("[MiyabiSkinPatch] 特殊挑战面板已存在，复用");
            return _funPilePanel;
        }

        GD.Print("[MiyabiSkinPatch] 🔨 新建特殊挑战面板...");
        _funPilePanel = new MiyabiFunPilePanel();
        _funPilePanel.Name = "MiyabiFunPilePanel";
        _funPilePanel.Position = new Vector2(1460, 770);
        screen.AddChild(_funPilePanel);
        _funPileHandlerWired = false;
        GD.Print($"[MiyabiSkinPatch] 特殊挑战面板已添加到屏幕 | parent={_funPilePanel.GetParent()?.GetType().Name ?? "null"}");
        return _funPilePanel;
    }

    // ============================================================
    // 事件：难度
    // ============================================================

    private static void OnDifficultyChanged(int newLevel)
    {
        GD.Print($"[MiyabiSkinPatch] 难度变更: {newLevel}");
        MiyabiModConfig.CombatHardSelected = (MiyabiSelectedHard)newLevel;
    }

    private static void OnEnemyStrongerChanged(bool toggled)
    {
        GD.Print($"[MiyabiSkinPatch] 加强敌人变更: {toggled}");
        MiyabiModConfig.MiyabiEnemiesStronger = toggled;
    }

    // ============================================================
    // 事件：皮肤
    // ============================================================

    private static void OnSkinTypeChanged(SkinType newType)
    {
        GD.Print($"[MiyabiSkinPatch] 皮肤类型变更: {newType}");
        _currentSkinType = newType;
        if (_skinPanel == null || !GodotObject.IsInstanceValid(_skinPanel)) return;
        LoadSkinData(_skinPanel, newType);
    }

    private static void OnSkinChanged(int newIndex)
    {
        GD.Print($"[MiyabiSkinPatch] 皮肤槽位变更: {newIndex} | type={_currentSkinType}");
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
        GD.Print($"[MiyabiSkinPatch] 特殊模式变更: {newIndex}");
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
        GD.Print($"[MiyabiSkinPatch] LoadSkinData 完成 | type={skinType} | count={skinCount} | slot={currentSlot}");
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

        string bgPath = miyabi.CustomCharacterSelectBgPath;
        var scene = ResourceLoader.Load<PackedScene>(bgPath);
        if (scene != null)
        {
            var control = scene.Instantiate<Control>(PackedScene.GenEditState.Disabled);
            control.Name = miyabi.Id.Entry + "_bg";
            bgContainer.AddChild(control);
        }
    }
}
