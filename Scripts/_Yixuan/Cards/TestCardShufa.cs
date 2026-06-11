using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class TestCardShufa : YixuanCardBase
    {
        public TestCardShufa()
            : base(0,  CardType.Attack, CardRarity.None, TargetType.AnyEnemy,false)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ShufaZhi>(choiceContext, cardPlay.Target, 60m, Owner.Creature, this);
        }
    }
}
