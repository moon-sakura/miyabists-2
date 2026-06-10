using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    //[RegisterPower]
    internal class ShannengPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        private const int MaxPoints = 151; // 上限为 150

        public override int DisplayAmount => Amount - 1;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/shanneng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Used",0),
        ];

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Amount > MaxPoints)
            {
                //Amount = MaxPoints;
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                // 确保不为负数
                //Amount = 1;
                SetAmount(1);
            }
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power != this) return;
            if (base.Amount > MaxPoints)
            {
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                // 确保不为负数
                SetAmount(1);
            }
        }


        public bool CanUseShanneng(int a)
        {
            if (a <= DisplayAmount) return true;
            return false;
        }

        public async Task UseShanneng(PlayerChoiceContext choiceContext, int a)
        {
            if (!CanUseShanneng(a)) return;

            // 使用后减少杀能点数
            SetAmount(base.Amount - a);
            DynamicVars["Used"].BaseValue += a;

            if (DynamicVars["Used"].BaseValue > MaxPoints - 1)
            {
                CardModel reward1 = base.Owner.CombatState.CreateCard<FufaQianchong>(base.Owner.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner.Player, CardPilePosition.Random);
            }
        }

        public void SetUsed(int used)
        {
            DynamicVars["Used"].BaseValue = used;
        }
    }
}
