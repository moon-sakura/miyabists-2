using MegaCrit.Sts2.Core.Entities.Cards;
using Miyabists2.Scripts;
using STS2RitsuLib.Content;
using System.Xml;

[RegisterOwnedCardKeyword(nameof(LieShuang), IconPath = null, CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
[RegisterOwnedCardKeyword(nameof(ExhaustX), IconPath = null, CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None)]
[RegisterOwnedCardKeyword(nameof(Friends), IconPath = null, CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(OtherWorldFriends), IconPath = null, CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(EndSkill), IconPath = null, CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class MiyabiKeywords
{
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    //[KeywordProperties(AutoKeywordPosition.Before)]
    public static readonly CardKeyword LieShuang = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(LieShuang)).GetModCardKeyword();

    public static readonly CardKeyword ExhaustX = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(ExhaustX)).GetModCardKeyword();

    public static readonly CardKeyword Friends = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Friends)).GetModCardKeyword();

    public static readonly CardKeyword OtherWorldFriends = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(OtherWorldFriends)).GetModCardKeyword();

    public static readonly CardKeyword EndSkill = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(EndSkill)).GetModCardKeyword();

}