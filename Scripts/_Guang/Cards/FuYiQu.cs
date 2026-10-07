using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 拂衣去 - 罕见技能卡
    /// 获得1层残影，抽1张牌。附加：消耗。
    /// 升级：去掉消耗。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class FuYiQu : GuangCardBase
    {
        public FuYiQu() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/fuYiQu.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Afterimage", 1),
            new CardsVar(1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // ========== 待实现 ==========
            // TODO: 获得 DynamicVars["Afterimage"] 层残影
            // 残影能力本项目暂未确认对应实现，待补充。

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust); // 升级后去掉消耗
        }
    }
}
