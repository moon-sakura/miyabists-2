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
    /// 格挡 - 初始技能卡
    /// 获得6点格挡。被消耗时获得1点敏捷。
    /// 升级：9点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class GeDang : GuangCardBase
    {
        public GeDang() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/geDang.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(6, ValueProp.Move),
            new DynamicVar("DexterityOnExhaust", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DexterityPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
        }

        // 被消耗时获得敏捷
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature,
                DynamicVars["DexterityOnExhaust"].IntValue, Owner.Creature, this);
        }
    }
}
