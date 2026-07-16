using System.Collections.Generic;

namespace Miyabists2.Scripts.UI;

/// <summary>
/// UI 本地化辅助类。支持 zhs / eng / kor / jpn，其他语言回退 eng。
/// </summary>
public static class MiyabiUILoc
{
    private static readonly Dictionary<string, string> Eng = new();
    private static readonly Dictionary<string, string> Zhs = new();
    private static readonly Dictionary<string, string> Kor = new();
    private static readonly Dictionary<string, string> Jpn = new();

    static MiyabiUILoc()
    {
        // ========== 进阶面板 ==========
        Add("diff_title",           "Advanced Options",  "进阶选项",      "고급 옵션",        "上級オプション");
        Add("diff_enemy_stronger",  "Stronger Enemies",  "加强模组敌人",  "강화된 적",        "強化された敵");
        Add("diff_none",            "None",              "无进阶",        "없음",             "なし");
        Add("diff_no_modifiers",    "No modifiers enabled", "未启用任何难度修正", "수정자가 활성화되지 않음", "修正は有効になっていません");

        Add("diff_lv1",  "[color=#FFD700]1.Sb[/color]：Enemies +10% HP",
                          "[color=#FFD700]1.锑级[/color]：敌人+10%生命值",
                          "[color=#FFD700]1.Sb[/color]：적 +10% 체력",
                          "[color=#FFD700]1.Sb[/color]：敵+10%HP");
        Add("diff_lv2",  "[color=#FFD700]2.Zn[/color]：At the start of each Act (except 1), [color=red]downgrade[/color] a random card",
                          "[color=#FFD700]2.锌级[/color]：每一幕（第一幕除外）开始时，随机[color=red]降级[/color]一张卡牌",
                          "[color=#FFD700]2.Zn[/color]：각 막 시작 시(1막 제외) 무작위 카드 [color=red]강등[/color]",
                          "[color=#FFD700]2.Zn[/color]：各幕開始時(第1幕以外)ランダムなカードを[color=red]ダウングレード[/color]");
        Add("diff_lv3",  "[color=#FFD700]3.Sn[/color]：First card played in combat costs [color=red]1 more[/color]",
                          "[color=#FFD700]3.锡级[/color]：战斗开始后，使用的第一张牌[color=red]费用+1[/color]",
                          "[color=#FFD700]3.Sn[/color]：전투 시작 후 첫 카드 [color=red]비용 +1[/color]",
                          "[color=#FFD700]3.Sn[/color]：戦闘開始後、最初に使用するカードの[color=red]コスト+1[/color]");
        Add("diff_lv4",  "[color=#FFD700]4.Mn[/color]：Enemy gains 1 Strength, you lose 1 Strength at combat start",
                          "[color=#FFD700]4.锰级[/color]：战斗开始时，敌人获得1点力量，自己失去1点力量",
                          "[color=#FFD700]4.Mn[/color]：전투 시작 시 적이 힘 1 획득, 자신은 힘 1 상실",
                          "[color=#FFD700]4.Mn[/color]：戦闘開始時、敵が筋力1獲得、自身は筋力1喪失");
        Add("diff_lv5",  "[color=#FFD700]5.Cd[/color]：Enemies +20% HP (replaces Sb)",
                          "[color=#FFD700]5.镉级[/color]：敌人改为+20%生命",
                          "[color=#FFD700]5.Cd[/color]：적 +20% 체력 (Sb 대체)",
                          "[color=#FFD700]5.Cd[/color]：敵+20%HP(Sb置換)");
        Add("diff_lv6",  "[color=#FFD700]6.Ni[/color]：[color=red]Draw 1 fewer card[/color] for the first 3 turns",
                          "[color=#FFD700]6.镍级[/color]：前三个回合[color=red]少抽1张牌[/color]",
                          "[color=#FFD700]6.Ni[/color]：첫 3턴 동안 [color=red]1장 적게 드로우[/color]",
                          "[color=#FFD700]6.Ni[/color]：最初の3ターン[color=red]ドローが1枚少ない[/color]");
        Add("diff_lv7",  "[color=#FFD700]7.Cu[/color]：Enemies deal 6 damage on death",
                          "[color=#FFD700]7.铜级[/color]：敌人在死亡时造成6点伤害",
                          "[color=#FFD700]7.Cu[/color]：적 사망 시 6 데미지",
                          "[color=#FFD700]7.Cu[/color]：敵が死亡時に6ダメージ");
        Add("diff_lv8",  "[color=#FFD700]8.Bi[/color]：Enemies +30% HP, gain +1 extra Strength",
                          "[color=#FFD700]8.铋级[/color]：敌人改为+30%生命，额外获得1点力量",
                          "[color=#FFD700]8.Bi[/color]：적 +30% 체력, 추가 힘 +1",
                          "[color=#FFD700]8.Bi[/color]：敵+30%HP、追加筋力+1");
        Add("diff_lv9",  "[color=#FFD700]9.Pb[/color]：Add a random [color=red]Curse[/color] to deck on entering a new Act",
                          "[color=#FFD700]9.铅级[/color]：进入新的一幕后会向卡组里加一张随机[color=red]诅咒[/color]",
                          "[color=#FFD700]9.Pb[/color]：새 막 진입 시 무작위 [color=red]저주[/color] 1장 추가",
                          "[color=#FFD700]9.Pb[/color]：新しい幕に入るとランダムな[color=red]呪い[/color]を1枚追加");
        Add("diff_lv10", "[color=#FFD700]10.Hg[/color]：Before first lethal damage, enemy gains 1 [color=#FFD700]Buffer[/color], 2 [color=#FFD700]Strength[/color] and recovers 30% HP",
                          "[color=#FFD700]10.汞级[/color]：敌人第一次受到致命伤害前会获得1层[color=#FFD700]缓冲[/color]与2点[color=#FFD700]力量[/color]并恢复30%生命",
                          "[color=#FFD700]10.Hg[/color]：첫 치명적 피해 전 적이 [color=#FFD700]완충[/color] 1, [color=#FFD700]힘[/color] 2 획득 및 30% 체력 회복",
                          "[color=#FFD700]10.Hg[/color]：最初の致命ダメージ前に敵が[color=#FFD700]バッファ[/color]1、[color=#FFD700]筋力[/color]2獲得しHP30%回復");

        // ========== 皮肤面板 ==========
        Add("skin_title",    "Skin Select",  "皮肤选择",   "스킨 선택",      "スキン選択");
        Add("skin_combat",   "Combat",       "Combat",     "Combat",         "Combat");
        Add("skin_rest",     "Rest",         "Rest",       "Rest",           "Rest");
        Add("skin_shop",     "Shop",         "Shop",       "Shop",           "Shop");

        // ========== 特殊挑战面板 ==========
        Add("funp_title", "Special Challenge", "特殊挑战", "특별 도전", "特殊挑戦");

        Add("funp_miyabi_default",   "Default：No changes",
                                      "默认：无变化",
                                      "기본：변화 없음",
                                      "デフォルト：変化なし");
        Add("funp_miyabi_bangboo",   "Bangboo Crew：Starting deck becomes Bangboo cards",
                                      "邦布当家：初始卡组变为邦布相关卡组",
                                      "Bangboo Crew：시작 덱이 Bangboo 카드로 변경",
                                      "ボンプ支配：初期デッキがボンプカードに");
        Add("funp_miyabi_bee",       "Swarm Assemble：Add 1 upgraded [color=#FFD700]Swarm Assemble[/color] to starting deck",
                                      "蜂群集结：初始卡组中添加1张升级后的[color=#FFD700]蜂群集结[/color]",
                                      "Swarm Assemble：시작 덱에 업그레이드된 [color=#FFD700]Swarm Assemble[/color] 1장 추가",
                                      "蜂集群集：初期デッキにアップグレード済み[color=#FFD700]蜂集群集[/color]を1枚追加");

        Add("funp_yixuan_default",   "Default：No changes",
                                      "默认：无变化",
                                      "기본：변화 없음",
                                      "デフォルト：変化なし");
        Add("funp_yixuan_bangboo",   "Bangboo Crew：Starting deck becomes Yixuan Bangboo cards",
                                      "邦布当家：初始卡组变为仪玄邦布相关卡组",
                                      "Bangboo Crew：시작 덱이 Yixuan Bangboo 카드로 변경",
                                      "ボンプ支配：初期デッキが儀玄ボンプカードに");
        Add("funp_yixuan_bee",       "Swarm Assemble：Add 1 upgraded [color=#FFD700]Swarm Assemble(Yixuan)[/color] to starting deck",
                                      "蜂群集结：初始卡组中添加1张升级后的[color=#FFD700]蜂群集结(仪玄)[/color]",
                                      "Swarm Assemble：시작 덱에 업그레이드된 [color=#FFD700]Swarm Assemble(Yixuan)[/color] 1장 추가",
                                      "蜂集群集：初期デッキにアップグレード済み[color=#FFD700]蜂集群集(儀玄)[/color]を1枚追加");

        Add("funp_miyabi_recorder",  "Ethereal Recorder：Start with a [color=#FFD700]Fairy Tale Notebook[/color]",
                                      "以骸记录者：游戏开始时获得一个[color=#FFD700]童话记事本[/color]",
                                      "Ethereal Recorder：시작 시 [color=#FFD700]동화 수첩[/color] 획득",
                                      "以骸記録者：ゲーム開始時に[color=#FFD700]童話手帳[/color]を入手");
        Add("funp_miyabi_grace",     "Grace's Revenge：Starting deck becomes Grace cards",
                                      "格莉丝的逆袭：初始卡组变为格莉丝卡组",
                                      "Grace's Revenge：시작 덱이 Grace 카드로 변경",
                                      "グレースの逆襲：初期デッキがグレースカードに");

        Add("funp_yixuan_recorder",  "Ethereal Recorder：Start with a [color=#FFD700]Fairy Tale Notebook[/color]",
                                      "以骸记录者：游戏开始时获得一个[color=#FFD700]童话记事本[/color]",
                                      "Ethereal Recorder：시작 시 [color=#FFD700]동화 수첩[/color] 획득",
                                      "以骸記録者：ゲーム開始時に[color=#FFD700]童話手帳[/color]を入手");
    }

    private static void Add(string key, string eng, string zhs, string kor, string jpn)
    {
        Eng[key] = eng;
        Zhs[key] = zhs;
        Kor[key] = kor;
        Jpn[key] = jpn;
    }

    /// <summary>
    /// 根据语言代码获取翻译文本。不支持的语言回退 eng。
    /// </summary>
    public static string Get(string key, string language)
    {
        var dict = language switch
        {
            "zhs" => Zhs,
            "kor" => Kor,
            "jpn" => Jpn,
            _ => Eng,
        };

        return dict.TryGetValue(key, out var value) ? value : Eng.GetValueOrDefault(key, key);
    }

    /// <summary>
    /// 规范化语言代码。
    /// </summary>
    public static string NormalizeLang(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "eng";
        return raw switch
        {
            "zhs" or "zht" or "zh" => "zhs",
            "kor" or "ko" => "kor",
            "jpn" or "ja" => "jpn",
            "eng" or "en" => "eng",
            _ => "eng",
        };
    }
}
