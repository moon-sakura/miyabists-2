using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    internal class ShuiNiao : MiyabiBlockCardBase
    {
        public override string PortraitPath => $"res://images/cards/shuiNiao.png";

        public override bool GainsBlock => false;

        public ShuiNiao() : base(1, CardRarity.Basic,true) { }

        protected override decimal GetExhaustUses()
        {
            return (int)MiyabiModConfig.CombatHardSelected >= 6 ? 4m : 2m;
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            //new DynamicVar(ParryVarName, 0),
            new DynamicVar(SlipperyVarName, 1),
            new DynamicVar(ExhaustCountVarName, GetExhaustUses()),
        ];

        public override void AfterCreated()
        {
            DynamicVars[ExhaustCountVarName].BaseValue = GetExhaustUses();
            base.AfterCreated();
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.ExhaustX
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            //HoverTipFactory.FromPower<MiyabiParryPower>(),
            //HoverTipFactory.FromCard<HuaCi>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
            //HoverTipFactory.FromKeyword(MiyabiKeywords.ExhaustX),
        ];

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            await TryExhaustAfterUse(context, cardPlay);
        }



        protected override void OnUpgrade()
        {
            //DynamicVars.Block.UpgradeValueBy(2);

            base.EnergyCost.UpgradeBy(-1);

            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
