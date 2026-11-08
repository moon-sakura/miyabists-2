using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Miyabists2.Scripts.Char;
using System;
using System.Reflection;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 明心镜状态下的战斗立绘切换。
    /// 进入状态：换成 <see cref="MingxinVisualsPath"/>；
    /// 退出状态：换回叶瞬光当前选择的战斗皮肤。
    ///
    /// 实现方式是直接替换战斗界面里 NCreature 的整个 NCreatureVisuals 节点。
    /// 明心镜场景与原战斗场景结构完全一致（Visuals / Bounds / IntentPos / CenterPos），
    /// 因此替换后 Hitbox、悬浮提示、意图位置都不需要重新计算，
    /// 这里仍然调用一次界面的 UpdateBounds 让布局保持同步。
    /// </summary>
    internal static class MingxinJingVisuals
    {
        /// <summary>明心镜状态下的战斗立绘场景。</summary>
        public const string MingxinVisualsPath = "res://scenes/_Guang/guang_char_mingxinjing.tscn";

        /// <summary>兜底立绘（默认战斗皮肤）。</summary>
        private const string DefaultVisualsPath = "res://scenes/_Guang/guang_char.tscn";

        private static readonly PropertyInfo? VisualsProperty =
            typeof(NCreature).GetProperty("Visuals", BindingFlags.Public | BindingFlags.Instance);

        private static readonly MethodInfo? UpdateBoundsMethod =
            typeof(NCreature).GetMethod(
                "UpdateBounds",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(Node) },
                null);

        /// <summary>进入明心镜状态：切换到明心镜立绘。</summary>
        public static void Enter(Creature? creature)
        {
            SwitchTo(creature, MingxinVisualsPath);
        }

        /// <summary>退出明心镜状态：换回当前选择的战斗皮肤。</summary>
        public static void Exit(Creature? creature)
        {
            if (creature?.Player?.Character is not YeshunGuang guang) return;

            string path = string.IsNullOrEmpty(guang.CombatDynamicVisualPath)
                ? DefaultVisualsPath
                : guang.CombatDynamicVisualPath;
            SwitchTo(creature, path);
        }

        private static void SwitchTo(Creature? creature, string path)
        {
            if (creature?.Player?.Character is not YeshunGuang) return;
            if (string.IsNullOrEmpty(path)) return;

            NCombatRoom? room = NCombatRoom.Instance;
            if (room == null) return;

            NCreature? node = room.GetCreatureNode(creature);
            if (node == null || !GodotObject.IsInstanceValid(node)) return;

            NCreatureVisuals? oldVisuals = node.Visuals;
            // 当前已经是目标场景时不重复替换。
            if (oldVisuals != null
                && GodotObject.IsInstanceValid(oldVisuals)
                && oldVisuals.SceneFilePath == path)
            {
                return;
            }

            if (VisualsProperty?.CanWrite != true) return;

            NCreatureVisuals? newVisuals = CreateVisuals(path);
            if (newVisuals == null) return;

            try
            {
                VisualsProperty.SetValue(node, newVisuals);
                node.AddChild(newVisuals);
                node.MoveChild(newVisuals, 0);
                newVisuals.Position = Vector2.Zero;

                if (oldVisuals != null && GodotObject.IsInstanceValid(oldVisuals))
                {
                    node.RemoveChild(oldVisuals);
                    oldVisuals.QueueFree();
                }

                UpdateBoundsMethod?.Invoke(node, new object[] { newVisuals });
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[MingxinJingVisuals] 切换立绘失败({path}): {ex.Message}");
            }
        }

        private static NCreatureVisuals? CreateVisuals(string path)
        {
            try
            {
                PackedScene? scene = PreloadManager.Cache.GetScene(path);
                if (scene == null) return null;
                return scene.Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[MingxinJingVisuals] 加载立绘场景失败({path}): {ex.Message}");
                return null;
            }
        }
    }
}
