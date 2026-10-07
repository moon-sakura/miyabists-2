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
    /// 照破无明 - 稀有能力的卡
    /// 效果暂定（与明心镜相关，待明心镜能力完成后补全）。
    /// 升级：固有。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhaoPoWuMing : GuangCardBase
    {
        public ZhaoPoWuMing() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhaoPoWuMing.png";

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // ========== 待实现 ==========
            // TODO: 效果暂定，待明心镜相关设计确定后补全。
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate); // 升级后固有
        }
    }
}
