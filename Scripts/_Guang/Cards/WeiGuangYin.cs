using MegaCrit.Sts2.Core.CardSelection;
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
    /// 微光引 - 稀有技能卡
    /// 从消耗堆中选择至多 2 张卡（终结技除外）加入抽牌堆，
    /// 将同等数量的《失感》加入弃牌堆。
    /// 升级：至多 3 张。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class WeiGuangYin : GuangCardBase
    {
        public WeiGuangYin() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MaxCards", 2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<ShiGan>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardPile exhaustPile = PileType.Exhaust.GetPile(Owner);

            // 终结技不能选
            bool HasSelectable(CardModel c) => !c.CanonicalKeywords.Contains(MiyabiKeywords.EndSkill);

            if (!exhaustPile.Cards.Any(HasSelectable)) return;

            // 至多 N 张（0 张也可以确认）
            List<CardModel> chosen = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                exhaustPile,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars["MaxCards"].IntValue),
                HasSelectable)).ToList();

            if (chosen.Count == 0) return;

            // 选中的卡加入抽牌堆
            foreach (CardModel card in chosen)
            {
                await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
            }

            // 将同等数量的《失感》加入弃牌堆
            for (int i = 0; i < chosen.Count; i++)
            {
                CardModel? curse = Owner.Creature.CombatState?.CreateCard<ShiGan>(Owner);
                if (curse == null) continue;

                await CardPileCmd.AddGeneratedCardToCombat(curse, PileType.Discard, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MaxCards"].UpgradeValueBy(1); // 升级后至多 3 张
        }
    }
}
