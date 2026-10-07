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
    /// 说明：明心镜能力尚未实现，本卡先只写框架。
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
            // ========== 待实现 ==========
            // TODO: 指定一张卡进入明心镜状态
            // 1. 弹出手牌选择界面选中一张卡
            //    CardModel? target = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
            // 2. 给该卡打上"明心镜"标记（具体实现待明心镜能力完成后接入）
        }

        protected override void OnUpgrade()
        {
            // ========== 待实现 ==========
            // TODO: 如果该卡不在手卡则加入手卡
        }
    }
}
