using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TestCardDaze : MiyabiCardBase
    {


        public TestCardDaze()
            : base(0, CardType.Attack, CardRarity.None, TargetType.AnyEnemy, false)
        {
        }


        // 通用打出逻辑
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await PowerCmd.Apply<DazePower>(cardPlay.Target, 50m, null, null);
        }
    }
}
