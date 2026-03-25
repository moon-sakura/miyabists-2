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
        //private const int MAX_STACKS = 100;
        //bool isUpdating = false;


        //public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        //{
        //    if (base.Owner != null && !base.Owner.IsDead && DisplayAmount >= MAX_STACKS && power == this && !isUpdating)
        //    {
        //        isUpdating = true;
        //        try
        //        {
        //            // 3. 执行修改。由于 isUpdating 是 true，
        //            // 这些 await 触发的次生 Hook 都会在第一行被拦截。
        //            //await PowerCmd.Apply<FrostBuildPower>(base.Owner, 1m, null, null,true);
        //            await PowerCmd.Apply<FrostPower>(base.Owner, 1m, null, null);
        //        }
        //        finally
        //        {

        //            isUpdating = false;
        //        }
        //    }
        //}

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            Amount++;
            // 检查是否达到100层
            //await CheckMaxStacks();
        }

        //private async Task ExecuteResetLogic()
        //{
        //    try
        //    {
        //        // 在这里处理真正耗时的修改
        //        await PowerCmd.SetAmount<FrostBuildPower>(base.Owner, 1m, null, null);
        //        await PowerCmd.Apply<FrostPower>(base.Owner, 1m, null, null);
        //    }
        //    finally
        //    {
        //        isUpdating = false;
        //    }
        //}


        //private async Task CheckMaxStacks()
        //{
        //    if (base.Owner != null && !base.Owner.IsDead && Amount >= MAX_STACKS + 1)
        //    {
        //        await PowerCmd.Apply<FrostPower>(base.Owner, 1m, null, null);
        //        //await PowerCmd.Apply<FrostBuildPower>(base.Owner, 1m - Amount, null, null);
        //        Amount = 1;
        //    }
        //}

    }
}
