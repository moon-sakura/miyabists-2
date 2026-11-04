using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 抱一 - 普通技能卡
    /// 抽1张卡，如果抽到的是伙伴卡，获得1点青溟剑势。
    /// 附加：被消耗时，选择1张被消耗的卡（终结技除外）加入手卡，本回合免费使用。
    /// 升级：抽2张。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class BaoYi : GuangCardBase, I2MingxinJing
    {
        public BaoYi() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(2),
            new DynamicVar("JianshiGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var drawnCards = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

            // 抽到的卡是伙伴卡则获得青溟剑势（升级后抽2张，每张伙伴卡各获得1点）
            int friends = drawnCards.Count(c => c.CanonicalKeywords.Contains(MiyabiKeywords.Friends));
            if (friends > 0)
            {
                await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                    friends * DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            //DynamicVars.Cards.UpgradeValueBy(1); // 升级后抽2张
            AddKeyword(CardKeyword.Retain); // 升级后保留)
        }

        // 被消耗时：选择1张被消耗的卡（终结技除外）加入手卡，本回合免费使用
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            CardPile exhaustPile = PileType.Exhaust.GetPile(Owner);

            bool HasSelectable(CardModel c) =>
                c != this && !c.CanonicalKeywords.Contains(MiyabiKeywords.EndSkill);

            if (!exhaustPile.Cards.Any(HasSelectable)) return;

            CardModel chosen = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                exhaustPile,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1),
                HasSelectable)).FirstOrDefault();

            if (chosen == null) return;

            await CardPileCmd.Add(chosen, PileType.Hand);
            chosen.SetToFreeThisTurn();
        }
    }
}
