using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class TestCardDazePlayer : MiyabiCardBase
    {
        public TestCardDazePlayer()
            : base(0, CardType.Attack, CardRarity.None, TargetType.AnyPlayer, false)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await MiyabiCombatService.DazeAddtoPlayer(choiceContext, Owner.Creature, 50);
        }
    }
}
