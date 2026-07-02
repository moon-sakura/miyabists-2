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
    private static readonly string[] YixuanSelectVoices = { "yixuan_go_buyigua", "yixuan_go_wolaizuozhen", "yixuan_select_ciciqianlai", "yixuan_select_xiaozaijiee" };

    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    public static void Postfix(CharacterModel characterModel)
    {
        // 只有选的是你的 Mod 角色时才播语音
        // 假设你的角色类名是 MiyabiCharacter
        if (characterModel is Miyabi)
        {
            // 从语音池随机播放
            MiyabiAudioPlay.Random(MiyabiSelectVoices, 1.2f);
        }
        if (characterModel is Yixuan)
        {
            // 从语音池随机播放
            MiyabiAudioPlay.Random(YixuanSelectVoices, 1.2f);
        }
    }
}