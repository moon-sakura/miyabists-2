using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Runs;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Patches
{
    /// <summary>
    /// 在 RunState 创建之前，向 modifiers 列表中注入自定义 Modifier。
    /// </summary>
    //[HarmonyPatch]
    //public static class ModifierInjectionPatch
    //{
    //    // ============================================================
    //    // 在这里返回你要注入的 modifier —— 替换成你自己的实例即可
    //    // ============================================================
    //    private static IEnumerable<ModifierModel> GetExtraModifiers(
    //        CharacterModel character,
    //        GameMode gameMode,
    //        int ascensionLevel)
    //    {
    //        // TODO: 替换下面这行为你自己的 modifier
    //        // yield return new YourModifierName();

    //        // 示例：按角色/模式/ascension 条件注入
    //        // if (character is Miyabi)
    //        //     yield return new MyMiyabiOnlyModifier();
    //        // if (ascensionLevel >= 10)
    //        //     yield return new HighAscensionModifier();
    //        if((character is Miyabi || character is Yixuan) || MiyabiModConfig.ChangeToAllPlayers)
    //        {
    //            var modifier = ModelDb.Modifier<MiyabiModModifier>().ToMutable();
    //            ((MiyabiModModifier)modifier).SetHard((int)MiyabiModConfig.CombatHardSelected);
    //            yield return modifier;
    //        }

    //        //yield break; // 目前为空，替换后删掉这行
    //    }

    //    // ==================== 单人 ====================

    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(NGame), nameof(NGame.StartNewSingleplayerRun))]
    //    private static void InjectSingleplayer(
    //        CharacterModel character,
    //        bool shouldSave,                          // 占位，不修改
    //        IReadOnlyList<ActModel> acts,             // 占位，不修改
    //        ref IReadOnlyList<ModifierModel> modifiers, // ← 要注入的参数
    //        string seed,
    //        GameMode gameMode,
    //        int ascensionLevel)
    //    {
    //        var extras = GetExtraModifiers(character, gameMode, ascensionLevel).ToList();
    //        if (extras.Count > 0)
    //        {
    //            modifiers = modifiers.Concat(extras).ToList();
    //        }
    //    }

    //    // ==================== 多人 ====================

    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(NGame), nameof(NGame.StartNewMultiplayerRun))]
    //    private static void InjectMultiplayer(
    //        StartRunLobby lobby,
    //        bool shouldSave,
    //        IReadOnlyList<ActModel> acts,
    //        ref IReadOnlyList<ModifierModel> modifiers, // ← 要注入的参数
    //        string seed,
    //        int ascensionLevel)
    //    {
    //        var extras = GetExtraModifiers(
    //            lobby.LocalPlayer.character, GameMode.Custom, ascensionLevel
    //        ).ToList();
    //        if (extras.Count > 0)
    //        {
    //            modifiers = modifiers.Concat(extras).ToList();
    //        }
    //    }
    //}
}
