using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers.CinimaPower
{
    internal class YixuanCinimasixPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.None;
        public override PowerStackType StackType => PowerStackType.Single;

        protected override bool IsVisibleInternal => false;

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card is QingmingYunying && cardPlay.Card.Owner.Creature == Owner)
            {
                CardModel reward1 = base.Owner.CombatState.CreateCard<FufaQianchong>(base.Owner.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner.Player, CardPilePosition.Random);
            }
        }
    }
}
