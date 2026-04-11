using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class ZhaojiaZhunbei : MiyabiBlockCardBase
    {
        public ZhaojiaZhunbei() : base(1, CardRarity.Common, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(10,ValueProp.Move),
            new DynamicVar(ParryVarName, 3)
        ];

        public override bool GainsBlock => false;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            PlayerCmd.EndTurn(base.Owner, false);
        }


        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
        }
    }
}
