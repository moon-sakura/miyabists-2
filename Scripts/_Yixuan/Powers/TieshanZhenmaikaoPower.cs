using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class TieshanZhenmaikaoPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/tieshankao.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("FreeCount", 0),
        ];

        public void AddFree(int amount)
        {
            DynamicVars["FreeCount"].BaseValue += amount;
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (dealer != Owner || cardSource == null)
                return 1m;

            if(cardSource.CanonicalKeywords.Any(k => k == MiyabiKeywords.Mingpo || k == MiyabiKeywords.Xuanmo)
                && cardSource.Owner.Creature == Owner)
            {
                return 1.2m;
            }
            return 1m;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.CanonicalKeywords.Any(k => k == MiyabiKeywords.Mingpo || k == MiyabiKeywords.Xuanmo)
                && cardPlay.Card.Owner.Creature == Owner)
            {
                await PowerCmd.Decrement(this);
            }

            if (cardPlay.Card.Type == CardType.Attack
                && cardPlay.Card.Owner.Creature == Owner
                && !cardPlay.IsAutoPlay
                && DynamicVars["FreeCount"].IntValue > 0)
            {
                DynamicVars["FreeCount"].BaseValue -= 1;
            }
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;

            if(card.Type == CardType.Attack
                && card.Owner.Creature == Owner
                && DynamicVars["FreeCount"].IntValue > 0)
            {
                modifiedCost = Math.Max(0m, originalCost - 1m);
                return true;
            }

            return false;
        }
    }
}
