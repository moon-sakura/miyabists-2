using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class EarsAll : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/earsAll.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<EarsLeft>(),
            HoverTipFactory.FromCard<EarsRight>(),
        ];

        public EarsAll()
            : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c.IsTransformable).ToList();
            foreach (CardModel card in list)
            {
                if (card.Type == CardType.Skill && !(card is EarsRight))
                {
                    CardModel card2 = base.Owner.Creature.CombatState?.CreateCard<EarsRight>(base.Owner);
                    await CardCmd.Transform(card, card2);
                }
                else if (card.Type == CardType.Attack && !(card is EarsLeft))
                {
                    CardModel card1 = base.Owner.Creature.CombatState?.CreateCard<EarsLeft>(base.Owner);
                    await CardCmd.Transform(card, card1);
                }
            }
        }
    }
}
