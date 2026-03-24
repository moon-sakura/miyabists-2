using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostBuildPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public override int DisplayAmount => Amount - 1;


        public string BigIconPath => "res://images/powers/FrostBuild.png";
        public string BigBetaIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomPackedIconPath => "res://images/powers/FrostBuild.png";
        public override string CustomBigIconPath => "res://images/powers/FrostBuild.png";


        // 最大堆叠层数常量
        private const int MAX_STACKS = 100;
        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckMaxStacks();
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount++;
            // 检查是否达到100层
            await CheckMaxStacks();
        }


        private async Task CheckMaxStacks()
        {
            if (DisplayAmount >= MAX_STACKS && base.Owner != null && !base.Owner.IsDead)
            {
                await PowerCmd.Apply<FrostPower>(base.Owner, 1m, null, null);
                await PowerCmd.SetAmount<FrostBuildPower>(base.Owner, 1m, null, null);
            }
        }

    }
}
