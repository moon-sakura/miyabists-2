using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class ChuijinDixin : MiyabiPartnerCardBase
    {
        public ChuijinDixin() : base(1, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/chuijinDixin.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(DazeVarName, 25),
            new DynamicVar(SupportVarName, 2),
            new DynamicVar("RongluCount", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<RongluShengwenPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 获得1层熔炉升温
            int rongluCount = DynamicVars["RongluCount"].IntValue;

            // 支援点数2：额外获得2层
            await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
            {
                rongluCount += 2;
            });

            await PowerCmd.Apply<RongluShengwenPower>(choiceContext, Owner.Creature, rongluCount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(10);
        }
    }
}
