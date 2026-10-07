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
    /// 不留行 - 初始技能卡
    /// 获得1点活力，3点格挡。
    /// 升级：2点活力，5点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class BuLiuXing : GuangCardBase
    {
        public BuLiuXing() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/buLiuXing.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Vigor", 1),
            new BlockVar(3, ValueProp.Move),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature,
                DynamicVars["Vigor"].IntValue, Owner.Creature, this);

            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Vigor"].UpgradeValueBy(1);
            DynamicVars.Block.UpgradeValueBy(2);
        }
    }
}
