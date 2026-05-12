using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class YixuanEnemyPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/yixuanEnemyPower.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private int HittedThisTurn = 0;
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("HittedTrigger", 5),
            new DynamicVar("DamageReduce", 80),
            new DynamicVar("DebuffReduce", 20),
        ];


        private bool phase1 = false;
        private bool phase2 = false;
        private bool phase3 = false;

        private bool isDoomTarget = false;
        private bool isPoisonedTarget = false;
        private bool isDebuffTarget = false;


        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target != base.Owner)
            {
                return;
            }
            await CheckPhase(choiceContext);
            HittedThisTurn++;
        }


        public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if(side == base.Owner.Side)
                HittedThisTurn = 0;

            return base.AfterTurnEnd(choiceContext, side);
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner)
            {
                return 1m;
            }
            if (HittedThisTurn >= DynamicVars["HittedTrigger"].IntValue)
            {
                return 1m - DynamicVars["DamageReduce"].IntValue / 100m;
            }

            return 1m;
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            await CheckPhase(choiceContext);

            //if (isDoomTarget)
            //{
            //    await PowerCmd.Apply<DoomPower>(choiceContext, base.Owner, -amount * (DynamicVars["DebuffReduce"].BaseValue / 100m), null, null);
            //    isDoomTarget = false;
            //}
            //if (isPoisonedTarget)
            //{
            //    await PowerCmd.Apply<PoisonPower>(choiceContext, base.Owner, -amount * (DynamicVars["DebuffReduce"].BaseValue / 100m), null, null);
            //    isPoisonedTarget = false;
            //}
            //if (isPoisonedTarget)
            //{
            //    await PowerCmd.Apply<PoisonPower>(choiceContext, base.Owner, -amount * (DynamicVars["DebuffReduce"].BaseValue / 100m), null, null);
            //    isDebuffTarget = false;
            //}
        }

        public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
        {
            if(canonicalPower.Type != PowerType.Debuff || target != base.Owner)
            {
                modifiedAmount = amount;
                return false;
            }

            modifiedAmount =amount * (1 - (DynamicVars["DebuffReduce"].BaseValue / 100m)) > 1 ? amount * (1 - (DynamicVars["DebuffReduce"].BaseValue / 100m)) : 1;
            return true;
        }

        //public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
        //{
        //    if (power is DoomPower && amount > 1 && target == base.Owner)
        //    {
        //        isDoomTarget = true;
        //    }
        //    if (power is PoisonPower && amount > 1 && target == base.Owner)
        //    {
        //        isPoisonedTarget = true;
        //    }
        //    if (power.Type == PowerType.Debuff && amount > 1 && target == base.Owner)
        //    {
        //        isDebuffTarget = true;
        //    }
        //}


        private async Task CheckPhase(PlayerChoiceContext context)
        {
            int doomAmount = base.Owner.GetPowerAmount<DoomPower>();
            int poisonAmount = base.Owner.GetPowerAmount<PoisonPower>();

            decimal percent = (decimal)base.Owner.CurrentHp / base.Owner.MaxHp;
            decimal posionPercent = (decimal)poisonAmount / base.Owner.MaxHp;
            decimal doomPercent = (decimal)doomAmount / base.Owner.MaxHp;

            if (percent <= 0.67m && !phase1)
            {
                await PowerCmd.Apply<IntangiblePower>(context, base.Owner, 1m, base.Owner, null);

                await PowerCmd.Apply<DoomPower>(context, base.Owner, -doomAmount/2, base.Owner, null);
                await PowerCmd.Apply<PoisonPower>(context, base.Owner, -poisonAmount/2, base.Owner, null);

                DynamicVars["DebuffReduce"].BaseValue = 50;

                foreach (Player player in Owner.CombatState.Players)
                {
                    PlayerCmd.EndTurn(player, false);
                }


                phase1 = true;
            }
            if (percent <= 0.34m && !phase2)
            {
                await PowerCmd.Apply<IntangiblePower>(context, base.Owner, 1m, base.Owner, null);

                await PowerCmd.Apply<DoomPower>(context, base.Owner, -doomAmount / 2, base.Owner, null);
                await PowerCmd.Apply<PoisonPower>(context, base.Owner, -poisonAmount / 2, base.Owner, null);

                DynamicVars["DebuffReduce"].BaseValue = 80;

                foreach (Player player in Owner.CombatState.Players)
                {
                    PlayerCmd.EndTurn(player, false);
                }


                phase2 = true;
            }
            if (percent <= 0.01m && !phase3)
            {
                await PowerCmd.Apply<IntangiblePower>(context, base.Owner, 1m, base.Owner, null);

                await PowerCmd.Apply<DoomPower>(context, base.Owner, -doomAmount / 2, base.Owner, null);
                await PowerCmd.Apply<PoisonPower>(context, base.Owner, -poisonAmount / 2, base.Owner, null);

                DynamicVars["DebuffReduce"].BaseValue = 100;


                foreach (Player player in Owner.CombatState.Players)
                {
                    PlayerCmd.EndTurn(player, false);
                }

                phase3 = true;
            }
        }

        
    }
}
