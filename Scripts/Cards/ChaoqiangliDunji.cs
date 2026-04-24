using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    internal class ChaoqiangliDunji : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/chaoqiangliDunji.png";

        public ChaoqiangliDunji() : base(1, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(11,ValueProp.Move),
            new DynamicVar(ParryVarName, 2),
            new DynamicVar(SupportVarName,3),
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
            
            await base.OnPlay(choiceContext, cardPlay);

            int parryCount = base.Owner.Creature.GetPower<MiyabiParryPower>()?.Amount ?? 0;

            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, parryCount));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, int parryCount)
        {
            await MiyabiCombatService.AddHuaCiReward(base.Owner.Creature, null, choiceContext, parryCount);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            //if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p)) p.UpgradeValueBy(1);

        }
    }
}
