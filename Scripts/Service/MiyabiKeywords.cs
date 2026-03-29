using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

public class MiyabiKeywords
{
    // 自定义枚举的名字。最终会变成{前缀}-{枚举值大写}的形式，例如TEST-UNIQUE
    [CustomEnum("LIESHUANG")]
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    //[KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword LieShuang;

    [CustomEnum("FRIENDS")]
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Friends;

    [CustomEnum("OTHERWORLDFRIENDS")]
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword OtherWorldFriends;

    [CustomEnum("ENDSKILL")]
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword EndSkill;
}