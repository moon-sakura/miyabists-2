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
    /// 逍遥云踪 - 罕见技能卡
    /// 获得 6 点格挡。
    /// 效果：获得 1 层弹回。
    /// 升级：8 点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class XiaoYaoYunZong : GuangCardBase
    {
        public XiaoYaoYunZong() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(6, ValueProp.Move),
            new DynamicVar("ReboundGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ReboundPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // 效果：获得 1 层弹回
            await PowerCmd.Apply<ReboundPower>(choiceContext, Owner.Creature,
                DynamicVars["ReboundGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2); // 升级后 8 点格挡
        }
    }
}
