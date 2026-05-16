
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics.SpecRelic
{
    public static class MiyabiRelicActiveService
    {
        // 1. 定义拦截原因（用于给玩家反馈）
        public enum BlockReason { None, Combat, CantDevour, AlreadyMaxed }

        private static readonly StringName HookedMetaKey = new StringName("is_devour_hooked");
        private static bool _isBusy = false;

        // 2. 注入入口：将此方法挂载到每个遗物图标生成的地方
        public static void WireRelicUI(Control holder, object relicData)
        {
            if (holder == null || holder.HasMeta(HookedMetaKey)) return;

            holder.SetMeta(HookedMetaKey, true);

            // 监听鼠标释放信号
            holder.Connect("gui_input", Callable.From<InputEvent>(inputEvent =>
            {
                OnRelicInput(inputEvent, relicData);
            }));
        }

        // 3. 输入判断：过滤右键点击
        private static void OnRelicInput(InputEvent inputEvent, object relicData)
        {
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                // ButtonIndex 2 通常是鼠标右键，!Pressed 表示按键弹起
                if (mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed && !_isBusy)
                {
                    HandleDevourLogic(relicData);
                }
            }
        }

        // 4. 核心逻辑：判断并执行效果
        private static async void HandleDevourLogic(object relicData)
        {
            _isBusy = true;
            try
            {
                // 这里假设 relicData 是你的遗物模型类
                // if (!IsEligible(relicData)) return; 

                GD.Print("检测到右键点击遗物，开始执行吃遗物逻辑...");

                // --- 执行你的自定义效果 ---
                // 例子：
                // await Player.Heal(10);
                // await RelicInventory.Remove(relicData);
                // await UI.ShowMessage("你吃掉了遗物，回复了10点血！");

                await Task.Delay(100); // 模拟异步操作
            }
            catch (Exception e)
            {
                GD.PrintErr($"执行失败: {e.Message}");
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
