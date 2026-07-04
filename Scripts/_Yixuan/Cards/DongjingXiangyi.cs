using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 动静相宜 - 2费Rare能力卡
    /// 使用青溟云影后，消耗所有闪能，恢复消耗闪能的50%数值
    /// 使用符法千重后，获得10点喧响值
    /// 升级后变为恢复70%与获得15点喧响值
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class DongjingXiangyi : YixuanCardBase
    {
        public DongjingXiangyi() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_YiXuan/cards/dongjingXiangyi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("ShannengRecoverPercent", 50),
            new DynamicVar("DecibelAmount", 10),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<QingmingYunying>(),
            HoverTipFactory.FromCard<FufaQianchong>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var power = await PowerCmd.Apply<DongjingXiangyiPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            power.SetAmounts(
                DynamicVars["ShannengRecoverPercent"].IntValue,
                DynamicVars["DecibelAmount"].IntValue
            );
        }

        protected override void OnUpgrade()
        {
            DynamicVars["ShannengRecoverPercent"].UpgradeValueBy(20);
            DynamicVars["DecibelAmount"].UpgradeValueBy(5);
        }
    }
}
