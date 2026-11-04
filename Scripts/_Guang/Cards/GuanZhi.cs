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
    /// 观止 - 罕见技能卡
    /// 指定一张卡进入明心镜状态。附加：消耗。
    /// 升级：如果该卡不在手卡则加入手卡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class GuanZhi : GuangCardBase
    {
        public GuanZhi() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/guanZhi.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 指定一张卡进入明心镜状态
            // 未升级：只能选手中已有可变换的卡；升级后：也可以从抽牌堆 / 弃牌堆里选
            PileType[] piles = [PileType.Hand, PileType.Draw, PileType.Discard];
                //IsUpgraded
                //? [PileType.Hand, PileType.Draw, PileType.Discard]
                //: [PileType.Hand];

            List<CardModel> options = piles
                .SelectMany(p => p.GetPile(Owner).Cards)
                //.Where(MingxinJingTransform.CanTransform)
                .ToList();

            if (options.Count == 0) return;

            CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
            if (chosen == null) return;

            bool wasInHand = PileType.Hand.GetPile(Owner).Cards.Contains(chosen);

            var cardUpgrade = chosen;

            if (MingxinJingTransform.CanTransform(chosen))
            {
                CardModel transformed = await MingxinJingTransform.Transform(chosen);
                if (transformed == null) return;
                cardUpgrade = transformed;
            }
            else
            {
                if(chosen.IsUpgradable)
                {
                    CardCmd.Upgrade(chosen,MegaCrit.Sts2.Core.Nodes.CommonUi.CardPreviewStyle.HorizontalLayout);
                }
                cardUpgrade = chosen;
            }

            // 升级后：原本不在手卡的卡，变换后加入手卡
            if (IsUpgraded && !wasInHand)
            {
                await CardPileCmd.Add(cardUpgrade, PileType.Hand.GetPile(Owner));
            }
        }

        protected override void OnUpgrade()
        {
            // 升级效果由 OnPlay 里的 IsUpgraded 分支处理：
            // 选择范围扩大到抽牌堆 / 弃牌堆，变换后的卡若不在手卡则加入手卡。
        }
    }
}
