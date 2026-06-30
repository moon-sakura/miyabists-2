using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ShimengKongxiang : YixuanPartnerCardBase
    {
        public ShimengKongxiang() : base(3, CardRarity.Rare, TargetType.Self, CardType.Power)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/shimengKongxiang.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("HpLossPercent", 10),
            new DynamicVar("DecibelAmount", 1),
            new DynamicVar("ShannengAmount", 20),
            new DynamicVar(SupportVarName, 1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShannengPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 应用能力：每回合失去生命、命破伤害次数+1、失去生命时获得喧响值
            await PowerCmd.Apply<ShimengKongxiangPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // 支援点数1：恢复20点闪能
            await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
            {
                await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, DynamicVars["ShannengAmount"].IntValue, Owner.Creature, this);
            });
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
