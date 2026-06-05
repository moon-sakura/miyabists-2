using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
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
    internal class TrainStepPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/training.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        int turnLimit = 2;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SetAmount(turnLimit);
        }
        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == base.Owner.Side)
            {
                Flash();
                if (Amount == 0)
                {
                    await PowerCmd.Remove(this);
                    return;
                }
                SetAmount(Amount - 1);
            }
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            PlayerChoiceContext choiceContext = new HookPlayerChoiceContext(oldOwner.Player, oldOwner.Player.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any);
            await PowerCmd.Apply<TrainStepOverPower>(choiceContext, oldOwner, 1, null, null);
            //return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player || cardPlay.Card.Type != CardType.Attack) return;

            CardModel cardModel = (await CardSelectCmd.FromHandForDiscard(context, base.Owner.Player, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
            if (cardModel != null)
            {
                await CardCmd.Discard(context, cardModel);
            }
        }
    }
}
