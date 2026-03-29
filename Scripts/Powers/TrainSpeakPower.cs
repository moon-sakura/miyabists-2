using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TrainSpeakPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        int turnLimit = 2;

        //类似眩晕机制
        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount = turnLimit;

            IEnumerable<CardModel> allCards = base.Owner.Player.PlayerCombatState.AllCards;
            foreach (CardModel item in allCards)
            {
                if (item.Affliction == null)
                {
                    await CardCmd.Afflict<Ringing>(item, 5m);
                }
            }
        }


        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Owner == base.Owner.Player && card.Affliction == null)
            {
                await CardCmd.Afflict<Ringing>(card, 5m);
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
                if (Amount == 0)
                {
                    await PowerCmd.Remove(this);
                    return;
                }
                Amount--;
            }
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            IEnumerable<CardModel> enumerable = oldOwner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
            foreach (CardModel item in enumerable)
            {
                if (item.Affliction is Ringing)
                {
                    CardCmd.ClearAffliction(item);
                }
            }
            await PowerCmd.Apply<TrainSpeakOverPower>(oldOwner, 1, null, null);
            //return Task.CompletedTask;
        }

        public override bool ShouldPlay(CardModel card, AutoPlayType _)
        {
            if (card.Owner.Creature != base.Owner)
            {
                return true;
            }
            if (!(card.Affliction is Ringing))
            {
                return true;
            }
            return CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card.Owner.Creature == base.Owner) <= 4 ;
        }

    }
}
