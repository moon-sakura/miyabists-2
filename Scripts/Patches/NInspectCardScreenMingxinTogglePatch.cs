using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Saves;
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 在卡牌阅览界面（NInspectCardScreen）的卡片右侧加一个「切换」按钮。
    /// 带有 I2MingxinJing 的原卡与对应的 GuangMingxinjingCardBase 之间可以来回预览。
    ///
    /// 切换时直接调用界面自己的 SetCard(index)，因此卡片下方的
    /// 「查看升级」勾选项会按照新卡片的可升级状态自动刷新，升级预览状态也会延续。
    /// </summary>
    [HarmonyPatch(typeof(NInspectCardScreen))]
    public static class NInspectCardScreenMingxinTogglePatch
    {
        /// <summary>借用卡牌图鉴排序按钮的样式（游戏内自带的金色标签按钮）。</summary>
        private const string ToggleButtonScenePath =
            "res://scenes/screens/card_library/library_sort_button.tscn";

        private static readonly FieldInfo CardsField =
            AccessTools.Field(typeof(NInspectCardScreen), "_cards");

        private static readonly FieldInfo IndexField =
            AccessTools.Field(typeof(NInspectCardScreen), "_index");

        private static readonly MethodInfo SetCardMethod =
            AccessTools.Method(typeof(NInspectCardScreen), "SetCard");

        /// <summary>每个阅览界面实例的切换状态。</summary>
        private static readonly ConditionalWeakTable<NInspectCardScreen, ToggleState> States = new();

        private sealed class ToggleState
        {
            public NCardViewSortButton? Button;

            /// <summary>当前按钮对应的是哪张卡。</summary>
            public int Index = -1;

            /// <summary>切换前该索引上的原卡。</summary>
            public CardModel? BaseCard;

            /// <summary>由原卡生成的另一种形态。</summary>
            public CardModel? PairedCard;

            public bool ShowingPaired;

            /// <summary>true 时说明正在由切换逻辑调用 SetCard，不要重置状态。</summary>
            public bool Swapping;

            public void Reset()
            {
                Index = -1;
                BaseCard = null;
                PairedCard = null;
                ShowingPaired = false;
            }
        }

        // ============================================================
        // Patch
        // ============================================================

        [HarmonyPostfix]
        [HarmonyPatch("_Ready")]
        public static void ReadyPostfix(NInspectCardScreen __instance)
        {
            ToggleState state = GetState(__instance);
            if (state.Button != null && GodotObject.IsInstanceValid(state.Button)) return;

            PackedScene? scene = PreloadManager.Cache.GetScene(ToggleButtonScenePath);
            if (scene == null) return;

            NCardViewSortButton button = scene.Instantiate<NCardViewSortButton>();
            if (button == null) return;

            // 放在卡片右侧、右箭头下方，避开下方的「查看升级」勾选项。
            // 以屏幕中心为锚点，适配不同分辨率。
            button.AnchorLeft = 0.5f;
            button.AnchorTop = 0.5f;
            button.AnchorRight = 0.5f;
            button.AnchorBottom = 0.5f;
            // 卡片悬浮提示从卡牌右侧（偏移 +353）开始向下堆叠，tips 一多会盖住右侧区域，
            // 所以把按钮放到卡片右下方、和「查看升级」同一行，并保证右边缘不超过 +344。
            // 「查看升级」那一行是 x=-122.5..122.5、y=450..514。
            button.OffsetLeft = 130f;
            button.OffsetTop = 450f;
            button.OffsetRight = 344f;
            button.OffsetBottom = 514f;
            button.Visible = false;

            __instance.AddChild(button);

            // 这是排序按钮，隐藏右上角的排序箭头，只保留文字。
            TextureRect? sortIcon = button.GetNodeOrNull<TextureRect>("%Image");
            if (sortIcon != null) sortIcon.Visible = false;

            button.Connect(
                NClickableControl.SignalName.Released,
                Callable.From<NClickableControl>(_ => OnTogglePressed(__instance, state)));

            state.Button = button;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Open")]
        public static void OpenPrefix(NInspectCardScreen __instance)
        {
            ToggleState state = GetState(__instance);

            // 上次停留在明心镜形态就再次打开（例如直接预览另一张卡）时，先把列表恢复原样。
            RestoreBaseCard(__instance, state);
            state.Reset();
            if (state.Button != null && GodotObject.IsInstanceValid(state.Button))
            {
                state.Button.Visible = false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Close")]
        public static void ClosePrefix(NInspectCardScreen __instance)
        {
            if (!States.TryGetValue(__instance, out ToggleState? state)) return;

            // 关闭时一定要把卡片列表还原，避免把明心镜预览卡留在调用方的列表里
            // （例如卡牌历史记录界面会直接复用它的 _allCards 列表）。
            RestoreBaseCard(__instance, state);
            state.Reset();

            if (state.Button != null && GodotObject.IsInstanceValid(state.Button))
            {
                state.Button.Visible = false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetCard")]
        public static void SetCardPostfix(NInspectCardScreen __instance)
        {
            ToggleState state = GetState(__instance);
            int index = GetIndex(__instance);

            // 通过左右箭头切到别的卡时，恢复原来的显示并清空切换状态。
            if (!state.Swapping && state.Index != index)
            {
                RestoreBaseCard(__instance, state);
                state.Reset();
                state.Index = index;
            }

            UpdateButton(__instance, state);
        }

        // ============================================================
        // 逻辑
        // ============================================================

        private static ToggleState GetState(NInspectCardScreen screen)
        {
            if (!States.TryGetValue(screen, out ToggleState? state))
            {
                state = new ToggleState();
                States.Add(screen, state);
            }
            return state;
        }

        private static void OnTogglePressed(NInspectCardScreen screen, ToggleState state)
        {
            if (state.Button == null || !GodotObject.IsInstanceValid(state.Button)) return;
            if (SetCardMethod == null) return;
            if (!screen.Visible) return;

            List<CardModel>? cards = GetCards(screen);
            int index = GetIndex(screen);
            if (cards == null || index < 0 || index >= cards.Count) return;

            CardModel current = cards[index];
            CardModel target;

            if (!state.ShowingPaired)
            {
                // 第一次点击：记录原卡，生成另一种形态。
                state.BaseCard = current;
                state.PairedCard = MingxinJingTransform.CreateDisplayCounterpart(current);
                if (state.PairedCard == null) return;

                target = state.PairedCard;
                state.ShowingPaired = true;
            }
            else
            {
                // 再点一次：切回原卡。
                if (state.BaseCard == null) return;
                target = state.BaseCard;
                state.ShowingPaired = false;
            }

            cards[index] = target;

            // 复用界面自己的 SetCard：会刷新箭头、下方升级勾选项以及卡面显示。
            state.Swapping = true;
            try
            {
                SetCardMethod.Invoke(screen, new object[] { index });
            }
            finally
            {
                state.Swapping = false;
            }

            UpdateButton(screen, state);
        }

        private static void RestoreBaseCard(NInspectCardScreen screen, ToggleState state)
        {
            if (!state.ShowingPaired || state.BaseCard == null) return;

            List<CardModel>? cards = GetCards(screen);
            if (cards == null) return;
            if (state.Index < 0 || state.Index >= cards.Count) return;

            cards[state.Index] = state.BaseCard;
        }

        private static void UpdateButton(NInspectCardScreen screen, ToggleState state)
        {
            NCardViewSortButton? button = state.Button;
            if (button == null || !GodotObject.IsInstanceValid(button)) return;

            CardModel? card = GetCurrentCard(screen);
            if (card == null || !screen.Visible)
            {
                button.Visible = false;
                return;
            }

            bool toMingxin = MingxinJingTransform.CanTransform(card);
            bool toOriginal = MingxinJingTransform.CanRevert(card);
            if (!toMingxin && !toOriginal)
            {
                button.Visible = false;
                return;
            }

            button.Visible = true;

            string language = GetLanguage();
            string text = toMingxin
                ? MiyabiUILoc.Get("mingxin_toggle_to_mingxin", language)
                : MiyabiUILoc.Get("mingxin_toggle_to_original", language);
            button.SetLabel(text);
        }

        private static List<CardModel>? GetCards(NInspectCardScreen screen)
        {
            return CardsField.GetValue(screen) as List<CardModel>;
        }

        private static CardModel? GetCurrentCard(NInspectCardScreen screen)
        {
            List<CardModel>? cards = GetCards(screen);
            int index = GetIndex(screen);
            if (cards == null || index < 0 || index >= cards.Count) return null;
            return cards[index];
        }

        private static int GetIndex(NInspectCardScreen screen)
        {
            object? value = IndexField.GetValue(screen);
            return value is int index ? index : -1;
        }

        private static string GetLanguage()
        {
            try
            {
                return SaveManager.Instance != null
                    ? MiyabiUILoc.NormalizeLang(SaveManager.Instance.SettingsSave.Language)
                    : "eng";
            }
            catch
            {
                return "eng";
            }
        }
    }
}
