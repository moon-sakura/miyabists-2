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
            // 指定一张手卡进入明心镜状态
            // 目前只在手牌中挑选；如果要支持其它牌堆，把 PileType.Hand 换成对应牌堆即可
            List<CardModel> options = PileType.Hand.GetPile(Owner).Cards
                .Where(MingxinJingTransform.CanTransform)
                .ToList();

            if (options.Count == 0) return;

            CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
            if (chosen != null)
            {
                await MingxinJingTransform.Transform(choiceContext, chosen);
            }
        }

        protected override void OnUpgrade()
        {
            // TODO: 如果该卡不在手卡则加入手卡
            // 需要把选择范围从手牌扩大到对应牌堆，并在变换后把明心镜卡加入手卡。
        }
    }
}
