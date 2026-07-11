using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class HollowGuodujinshe : MiyabiCardBase
    {
        // protected override string ArtPath => "res://images/cards/hollowErosion.png";
        protected override string ArtPath => "res://images/cards/hollow.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Unplayable,
            CardKeyword.Retain,
        ];

        public HollowGuodujinshe()
            : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        public override int MaxUpgradeLevel => 0;

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == Owner.Creature.Side && Pile.Type == PileType.Hand)
            {
                await CreatureCmd.GainBlock(Owner.Creature, 4, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, null);
            }
        }
    }
}
