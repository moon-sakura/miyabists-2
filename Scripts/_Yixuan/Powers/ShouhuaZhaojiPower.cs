using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    /// <summary>
    /// 守华照寂：获得格挡时变为0格挡，然后获得1点能量，抽1张卡
    /// </summary>
    internal class ShouhuaZhaojiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        //public string BigIconPath => "res://images/_YiXuan/powers/shouhuaZhaoji.png";
        //public string BigBetaIconPath => BigIconPath;
        //public override string CustomIconPath => BigIconPath;
        //public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(0),
        ];

        public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
        {
            if(target == Owner && block >= 0)
                return 0m;

            return 1m;
        }

        public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
        {
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
            await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), Amount, Owner.Player);
        }

        public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            DynamicVars.Energy.BaseValue = Amount;
            return base.AfterCardPlayed(choiceContext, cardPlay);
        }
    }
}
