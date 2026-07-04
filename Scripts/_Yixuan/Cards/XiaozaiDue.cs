using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 消灾渡厄 - 1费Uncommon能力卡
    /// 每使用10张卡，将一张符法千重·破加入手卡
    /// 升级后添加固有
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class XiaozaiDue : YixuanCardBase
    {
        public XiaozaiDue() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_YiXuan/cards/xiaozaiDue.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CardsPerTrigger", 10),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<FufaQianchongPo>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<XiaozaiDuePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
