using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
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

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CardCount", 0)
        ];

        private int FHC = 0; // FengHua Count, 用于记录本回合中打出的风花数量

        //public override int DisplayAmount => Amount - 1;


        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player || !(cardPlay.Card is FengHua)) return;

            //await PowerCmd.SetAmount<TunafaPower>(Owner, Amount + 1, null, null);
            DynamicVars["CardCount"].BaseValue += 1;

            if (DynamicVars["CardCount"].IntValue == 4) 
                Flash();

            if(DynamicVars["CardCount"].IntValue >= 5) 
            {
                await PowerCmd.Apply<FrostFallPower>(Owner, Amount, Owner, null);
                DynamicVars["CardCount"].BaseValue = 0;
            }
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (!(card is FengHua) || FHC >= Amount)
            {
                return false;
            }

            // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
            modifiedCost = 0m;
            FHC++;
            return true;
        }
    }
}
