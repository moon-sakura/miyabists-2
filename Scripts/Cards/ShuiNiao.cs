using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    internal class ShuiNiao : MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/shuiNiao.png";

        public override bool GainsBlock => false;

        public ShuiNiao() : base(1, CardType.Skill,CardRarity.Basic, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(ParryVarName, 0),
            new DynamicVar(SlipperyVarName, 1),
            new DynamicVar("ExhaustCount", (int)MiyabiModConfig.CombatHardSelected >= 6 ? 4 : 2),
        ];

        public override void AfterCreated()
        {
            DynamicVars["ExhaustCount"].BaseValue = Count();
            base.AfterCreated();
        }

        decimal Count() => (int)MiyabiModConfig.CombatHardSelected >= 6 ? 4m : 2m;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            //HoverTipFactory.FromPower<MiyabiParryPower>(),
            //HoverTipFactory.FromCard<HuaCi>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
            //HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        ];

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if(cardPlay.Card == this)
            {
                DynamicVars["ExhaustCount"].BaseValue -= 1;

                if (DynamicVars["ExhaustCount"].BaseValue <= 0)
                {
                    await CardCmd.Exhaust(context, this);
                    DynamicVars["ExhaustCount"].BaseValue = (int)MiyabiModConfig.CombatHardSelected >= 6 ? 4 : 2;
                }
            }
        }

        

        protected override void OnUpgrade()
        {
            //DynamicVars.Block.UpgradeValueBy(2);

            base.EnergyCost.UpgradeBy(-1); 

            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
