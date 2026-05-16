using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    internal class ModConfigEffect : CustomPowerModel
    {
        public override PowerType Type => PowerType.None;
        public override PowerStackType StackType => PowerStackType.Single;

        protected override bool IsVisibleInternal => false;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(dealer == null) return 1m;

            if(dealer == Owner && props.IsPoweredAttack())
            {
                if (dealer.Player.Character is Miyabi) return (decimal)MiyabiModConfig.DamageDealtLimit;

                if (dealer.IsPlayer && MiyabiModConfig.ChangeToAllPlayers) return (decimal)MiyabiModConfig.DamageDealtLimit;
            }

            if(dealer.IsEnemy && target == Owner)
            {
                if (target.Player.Character is Miyabi) return (decimal)MiyabiModConfig.DamageTakenMultiplier;

                if (MiyabiModConfig.ChangeToAllPlayers) return (decimal)MiyabiModConfig.DamageTakenMultiplier;
            }

            return 1m;
        }
    }


    [HarmonyPatch(typeof(Hook), "BeforeCombatStart")]
    internal static class ModPatch
    {
        private static async void Postfix(CombatState combatState, Task __result)
        {
            foreach (var player in combatState.Players)
            {
                await PowerCmd.Apply<ModConfigEffect>(new ThrowingPlayerChoiceContext(),player.Creature, 1, null, null);
            }
        }
    }

    [HarmonyPatch(typeof(CombatState), "CreateCreature")]
    internal static class MonsterMaxHpPatch
    {
        private static void Postfix(Creature __result, CombatSide side)
        {
            if (side == CombatSide.Enemy && (__result.CombatState.Players.Any(c => c.Character is Miyabi) || MiyabiModConfig.ChangeToAllPlayers))
            {
                int num = (int)Math.Ceiling(__result.MaxHp * (decimal)MiyabiModConfig.MonsterHpMax);
                //int num2 = __result.MaxHp + num;
                __result.SetMaxHpInternal(num);
                __result.SetCurrentHpInternal(num);
            }
        }
    }

}
