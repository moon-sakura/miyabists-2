using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class MiyabiBossPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/miyabiFull.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DeathPrevent", 1),
        ];

        public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(MiyabiModConfig.MiyabiEnemiesStronger)
            {
                DynamicVars["DeathPrevent"].BaseValue = 2;
            }
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(applier != base.Owner || !(power is BreakPlayerPower || power is DisorderPower) || amount < 1)
            {
                return;
            }

            if(power is BreakPlayerPower)
            {
                foreach(var powerItem in base.Owner.Powers)
                {
                    if(powerItem.Type == PowerType.Debuff) 
                    {
                        await PowerCmd.Remove(powerItem);
                    }
                }
            }

            if(power is DisorderPower)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 2m, base.Owner, null);
            }
        }

        public override bool ShouldDie(Creature creature)
        {
            if(creature == base.Owner && DynamicVars["DeathPrevent"].IntValue > 0)
            {
                DynamicVars["DeathPrevent"].BaseValue -= 1;
                
                return false;
            }
            return base.ShouldDie(creature);
        }
        public override async Task AfterPreventingDeath(Creature creature)
        {
            if(creature == base.Owner)
            {
                foreach (var powerItem in base.Owner.Powers)
                {
                    if (powerItem.Type == PowerType.Debuff)
                    {
                        await PowerCmd.Remove(powerItem);
                    }
                }
            }
        }


        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (dealer == base.Owner && target.IsPlayer && target.IsAlive)
            {
                int dazeMulti = MiyabiModConfig.MiyabiEnemiesStronger ? 5 : 3;
                await MiyabiCombatService.DazeAddtoPlayer(choiceContext, target, result.UnblockedDamage * dazeMulti);

                ///
                ///
                ///

                MiyabiCombatService.SetFrostTriggerMultiply(base.Owner);
                int trigger = MiyabiModConfig.MiyabiEnemiesStronger ? 60 : 50;

                int chkFB = target.GetPowerAmount<FrostBuildPower>() + result.TotalDamage;

                // 确保是本卡造成的实际伤害，且目标存活
                if (result.TotalDamage > 0 && chkFB <= trigger && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
                {
                    await PowerCmd.Apply<FrostBuildPower>(choiceContext, target, result.TotalDamage, base.Owner, null);
                }
                //烈霜积蓄值积攒逻辑
                if (chkFB >= trigger + 1)
                {
                    //await MiyabiCombatService.FrostApply(target,base.Owner.Creature,choiceContext);
                    //await PowerCmd.SetAmount<FrostBuildPower>(target, 1, base.Owner.Creature, this);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<FrostBuildPower>(), 1, dealer, null);
                    //await PowerCmd.Apply<FrostPower>(choiceContext, target, 1, base.Creature, null);
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 10m, ValueProp.Unpowered, (Creature)null);

                    //int fireAmount = target.GetPowerAmount<FrostFirePower>();
                    //await CreatureCmd.Damage(null, target, 10m, ValueProp.Unpowered, dealer);


                    if (target.HasPower<AttributeAnomalyPower>())
                    {
                        await MiyabiCombatService.DisorderApply(target, base.Owner, choiceContext);
                    }
                    else
                    {
                        await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, base.Owner, null);
                    }
                }
            }
        }
    }
}
