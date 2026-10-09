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
    /// 归去时 - 罕见技能卡
    /// 获得8点格挡，下回合额外抽1张卡。
    /// 升级：10点格挡，下回合额外抽2张卡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class GuiQuShi : GuangCardBase
    {
        public GuiQuShi() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/guiQuShi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(8, ValueProp.Move),
            new DynamicVar("DrawNextTurn", 1),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // 下回合额外抽牌（DrawCardsNextTurnPower）
            await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature,
                DynamicVars["DrawNextTurn"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            DynamicVars["DrawNextTurn"].UpgradeValueBy(1);
        }
    }
}
