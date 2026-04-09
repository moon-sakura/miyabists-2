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
    internal class QuanmianQingchang : MiyabiPartnerCardBase
    {
        //public override string PortraitPath => $"res://images/cards/baojunMengji.png";

        public QuanmianQingchang() : base(1, CardRarity.Common, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(11,ValueProp.Move),
            new DynamicVar(SupportVarName, 2)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            if (base.DynamicVars.TryGetValue(SupportVarName, out DynamicVar s))
                await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature, s.IntValue, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(SupportVarName, out DynamicVar s)) s.UpgradeValueBy(1);

        }
    }
}
