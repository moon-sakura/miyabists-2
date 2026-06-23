using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class BestPartner : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/manhanQuanxi.png";

        public BestPartner() : base(1, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(6,ValueProp.Move),
            new DynamicVar(SupportVarName,1),
        ];

        public override bool GainsBlock => true;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            await PowerCmd.Apply<BestPartnerPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext)
        {
            await CreatureCmd.Heal(Owner.Creature, 3m);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            //if (base.DynamicVars.TryGetValue(SupportVarName, out DynamicVar s)) s.UpgradeValueBy(1);

        }
    }
}
