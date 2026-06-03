using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class EarsRight : MiyabiAttackCardBase
    {
        protected override string ArtPath => $"res://images/cards/earsAll.png";

        public EarsRight() : base(1, CardRarity.Token, TargetType.AnyEnemy, true) { }

        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(10, ValueProp.Move),
            new DynamicVar("ANOBUILD_POWER", 1),
            new BlockVar(10, ValueProp.Move),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<FrostPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);
            await MiyabiCombatService.AddAnoBuildup(cardPlay.Target, base.DynamicVars["ANOBUILD_POWER"].IntValue, base.Owner.Creature, this, choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(5);
            //DynamicVars.Block.UpgradeValueBy(2);
            DynamicVars["ANOBUILD_POWER"].UpgradeValueBy(1);
        }
    }
}
