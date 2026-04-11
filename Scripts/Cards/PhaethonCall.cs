using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class PhaethonCall : MiyabiCardBase
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
            HoverTipFactory.FromKeyword(MiyabiKeywords.OtherWorldFriends)
        ];

        public PhaethonCall()
            : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            List<CardModel> cardModel = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                                                                               where (c.CanonicalKeywords.Contains(MiyabiKeywords.Friends) || c.CanonicalKeywords.Contains(MiyabiKeywords.OtherWorldFriends)) 
                                                                               && (c.Rarity != CardRarity.Token || c.Rarity != CardRarity.None || c.Rarity != CardRarity.Curse || c.Rarity != CardRarity.Status)
                                                                               select c, 3, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
            CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, cardModel, base.Owner);
            if (chosen != null)
            {
                //cardModel.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, addedByPlayer: true);
            }
        }

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
    }
}
