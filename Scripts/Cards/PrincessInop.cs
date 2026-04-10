using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class PrincessInop : MiyabiCardBase
    {
        //protected override string ArtPath => "res://images/cards/SPxiaoye.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<ArtifactPower>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
        ];

        public PrincessInop()
            : base(0, CardType.Skill, CardRarity.None, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ArtifactPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            await PowerCmd.Apply<SlipperyPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
            await PowerCmd.Apply<PlatingPower>(base.Owner.Creature, 2, base.Owner.Creature, this);
        }
    }
}
