using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 止戈 - 普通技能卡
    /// 获得8点格挡，获得1层人工制品。附加：被消耗时下回合获得1点能量。
    /// 升级：11点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhiGe : GuangCardBase
    {
        public ZhiGe() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhiGe.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(8, ValueProp.Move),
            new DynamicVar("Artifact", 1),
            new EnergyVar(1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ArtifactPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature,
                DynamicVars["Artifact"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
        }

        // 被消耗时下回合获得能量
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature,
                DynamicVars.Energy.IntValue, Owner.Creature, this);
        }
    }
}
