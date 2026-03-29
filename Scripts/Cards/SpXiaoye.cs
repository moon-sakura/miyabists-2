using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class SpXiaoye : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/SPxiaoye.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.OtherWorldFriends];

        public SpXiaoye()
            : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<IntangiblePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            await PowerCmd.Apply<IntangiblePower>(cardPlay.Target, 1m, base.Owner.Creature, this);

            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 2);
            List<CardModel> cardsIn = base.Owner.PlayerCombatState.AllCards.ToList();
            if (cardsIn.Count != 0)
            {
                IEnumerable<CardModel> cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs));
                if (cardModel != null)
                {
                    foreach (CardModel c in cardModel) CardCmd.Exhaust(choiceContext, c);
                }
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
