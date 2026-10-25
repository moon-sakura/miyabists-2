using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 溯影惊鸿 - 稀有能力的卡
    /// 每次使用伙伴卡获得1点青溟剑势。
    /// 升级：固有。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class SuYingJingHong : GuangCardBase
    {
        public SuYingJingHong() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/suYingJingHong.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiPerPartner", 1),
            new EnergyVar(3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<SuYingJingHongPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiPerPartner"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate); // 升级后固有
        }
    }
}
