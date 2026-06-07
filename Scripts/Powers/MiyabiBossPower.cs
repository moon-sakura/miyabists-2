using STS2RitsuLib.Interop.AutoRegistration;
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
    internal class MiyabiBossPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/miyabiFull.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DeathPrevent", 2),
        ];

        public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if(MiyabiModConfig.MiyabiEnemiesStronger)
            {
                DynamicVars["DeathPrevent"].BaseValue = 3;
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

        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner || amount < target.CurrentHp) return;

            GD.Print($"[MiyabiBoss] ShouldDie check for {target.Name}, DeathPrevent = {DynamicVars["DeathPrevent"].IntValue}");

            if (DynamicVars["DeathPrevent"].IntValue > 0)
            {
                GD.Print("[MiyabiBoss] Preventing death and removing one stack of DeathPrevent.");
                DynamicVars["DeathPrevent"].BaseValue -= 1;
                foreach (var powerItem in base.Owner.Powers.ToList())
                {
                    if (powerItem.Type == PowerType.Debuff)
                    {
                        await PowerCmd.Remove(powerItem);
                    }
                }
                decimal heal = Math.Max(1m, base.Owner.MaxHp);
                await CreatureCmd.Heal(base.Owner, heal);
                await PowerCmd.Apply<IntangiblePower>(choiceContext, base.Owner, 1m, base.Owner, null);
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
                if (result.TotalDamage > 0 && chkFB <= trigger)
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
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 10m, ValueProp.Unpowered & ValueProp.Unblockable, (Creature)null);

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

        //private bool _isHandlingDeath = false; // 状态锁
        //public override bool ShouldDie(Creature creature)
        //{
        //    if (creature != base.Owner) return true;

        //    // 如果正在处理免死回血期间，直接返回 false，拒绝重复检测
        //    if (_isHandlingDeath) return false;

        //    GD.Print($"[MiyabiBoss] ShouldDie check for {creature.Name}, DeathPrevent = {DynamicVars["DeathPrevent"].IntValue}");

        //    if (DynamicVars["DeathPrevent"].IntValue > 0)
        //    {
        //        GD.Print("[MiyabiBoss] Preventing death and removing one stack of DeathPrevent.");
        //        DynamicVars["DeathPrevent"].BaseValue -= 1;
        //        _isHandlingDeath = true; // 上锁

        //        return false;
        //    }
        //    return true;
        //}

        //public override async Task AfterPreventingDeath(Creature creature)
        //{
        //    if (creature == base.Owner)
        //    {
        //        //foreach (var powerItem in base.Owner.Powers)
        //        //{
        //        //    if (powerItem.Type == PowerType.Debuff)
        //        //    {
        //        //        await PowerCmd.Remove(powerItem);
        //        //    }
        //        //}
        //        decimal amount = Math.Max(1m, base.Owner.MaxHp);
        //        await CreatureCmd.Heal(base.Owner, amount);
        //        _isHandlingDeath = false; // ⬇️ 解锁，恢复正常的死亡检测机制
        //        GD.Print($"[MiyabiBoss] AfterPreventingDeath triggered for {creature.Name}");
        //    }
        //}
    }
}
