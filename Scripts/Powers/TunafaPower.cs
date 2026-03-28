using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Cards;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TunafaPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public int CardsCount { get; set; } = 0;


        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player || !(cardPlay.Card is ShuangYue)) return;

            CardsCount += 1;

            if (CardsCount == 4) 
                Flash();

            if(CardsCount >= 5) 
            {
                await PowerCmd.Apply<FrostFallPower>(Owner, Amount, Owner, null);
                CardsCount = 0;
            }
        }
    }
}
