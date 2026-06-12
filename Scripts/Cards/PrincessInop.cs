using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class PrincessInop : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/princessInop.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        public override int MaxUpgradeLevel => 0;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<ArtifactPower>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),
        ];

        public PrincessInop()
            : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ArtifactPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
            await PowerCmd.Apply<SlipperyPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
            await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, 2, base.Owner.Creature, this);
            await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
            await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, 2, base.Owner.Creature, this);
        }
    }
}
