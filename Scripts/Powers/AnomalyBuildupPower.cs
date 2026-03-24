using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class AnomalyBuildupPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public override int DisplayAmount => Amount - 1;

        public int _triggerAmount = 5;

        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount++;
            await CheckAno();
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckAno();
        }

        public async Task CheckAno()
        {
            if (base.DisplayAmount < _triggerAmount) return;
            // 为敌人附加一次属性异常
            await PowerCmd.Apply<AttributeAnomalyPower>(base.Owner, 1, null, null);

            // 层数 -5
            await PowerCmd.Apply<AnomalyBuildupPower>(base.Owner, -_triggerAmount, null, null);
        } 
    }
}
