using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Char;


[HarmonyPatch]
public static class MiyabiCharSelectPatch
{
    private static readonly string[] MiyabiSelectVoices = { "select_ChueWujin", "select_JianxingCidao", "begin_Zhan", "begin_ZhunbeiBadao" };

    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    public static void Postfix(CharacterModel characterModel)
    {
        // 只有选的是你的 Mod 角色时才播语音
        // 假设你的角色类名是 MiyabiCharacter
        if (characterModel is Miyabi)
        {
            // 随机选一句
            int idx = (int)(GD.Randi() % MiyabiSelectVoices.Length);

            // 它会自动处理加载、播放、音量转换和自动销毁
            MiyabiAudioService.Play(MiyabiSelectVoices[idx],1.2f);

            GD.Print("[MiyabiMod] 播放选人语音: " + MiyabiSelectVoices[idx]);
        }
    }
}