using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class ChaoqiangliDunji : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/baojunMengji.png";

        public ChaoqiangliDunji() : base(1, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(11,ValueProp.Move),
            new DynamicVar(ParryVarName, 2)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int parryCount = base.Owner.Creature.GetPower<MiyabiParryPower>()?.Amount ?? 0;

            await base.OnPlay(choiceContext, cardPlay);

            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);


            if (base.CheckSupportCost(3) != 0)
            {
                if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p))
                    for (int i = 0; i < parryCount + p.IntValue; i++)
                    {
                        CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<HuaCi>(base.Owner);
                        await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
                    }
                await CostSupporPoint(3);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            //if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p)) p.UpgradeValueBy(1);

        }
    }
}
