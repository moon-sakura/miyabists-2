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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Miyabists2.Scripts.Powers
{
    internal class TrainAttackPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        int turnLimit = 2;
        int cardsLimit = 3;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount = turnLimit;

            IEnumerable<CardModel> allCards = base.Owner.Player.PlayerCombatState.AllCards;
            foreach (CardModel item in allCards)
            {
                if (item.Affliction == null)
                {
                    await CardCmd.Afflict<Ringing>(item, 3m);
                }
            }
        }


        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Owner == base.Owner.Player && card.Affliction == null)
            {
                await CardCmd.Afflict<Ringing>(card, 3m);
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
            await PowerCmd.Apply<TrainAttackOverPower>(oldOwner, 3, null, null);
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
            return CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card.Owner.Creature == base.Owner) < 3;
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (ShouldSkip(card))
            {
                return false;
            }

            // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
            modifiedCost = default(decimal);
            return true;
        }

        // 核心逻辑 2：修改星能/特殊消耗（保持逻辑一致）
        public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (ShouldSkip(card))
            {
                return false;
            }
            modifiedCost = default(decimal);
            return true;
        }

        private bool ShouldSkip(CardModel card)
        {
            bool flag = card.Owner.Creature != base.Owner;
            bool flag2 = flag;
            if (!flag2)
            {
                bool flag3;
                switch (card.Pile?.Type)
                {
                    case PileType.Hand:
                    case PileType.Play:
                        flag3 = true;
                        break;
                    default:
                        flag3 = false;
                        break;
                }
                flag2 = !flag3;
            }
            if (!flag2)
            {
                return false;//base.Amount >= cardsLimit;
            }
            return true;
        }
    }
}
