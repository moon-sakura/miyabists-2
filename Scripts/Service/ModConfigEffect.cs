using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
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

        private int playcount = 0;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if((int)MiyabiModConfig.CombatHardSelected >= 4 && (Owner.Player.Character is Miyabi || MiyabiModConfig.ChangeToAllPlayers))
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -1, null, null);
            }
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;

            if (card.Owner.Creature != base.Owner) return false;
            bool inHand = card.Pile?.Type == PileType.Hand || card.Pile?.Type == PileType.Play;
            if (!inHand) return false;

            if (playcount >= 1)
            {
                return false;
            }

            // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
            modifiedCost = originalCost + 1m;
            return true;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            playcount++;
            return base.AfterCardPlayed(choiceContext, cardPlay);
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if((int)MiyabiModConfig.CombatHardSelected >= 7 && (Owner.Player.Character is Miyabi || MiyabiModConfig.ChangeToAllPlayers))
            {
                if(player.Creature.CombatState.RoundNumber <= 3)
                    return base.ModifyHandDraw(player, count) - 1;
            }
            return base.ModifyHandDraw(player, count);
        }
    }

    internal class ModConfigEnemyEffect : CustomPowerModel
    {
        public override PowerType Type => PowerType.None;
        public override PowerStackType StackType => PowerStackType.Single;

        protected override bool IsVisibleInternal => false;

        public async override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
        {
            if(creature != base.Owner) return;

            if((int)MiyabiModConfig.CombatHardSelected >= 6 && (Owner.Player.Character is Miyabi || MiyabiModConfig.ChangeToAllPlayers))
            {
                foreach(var player in creature.CombatState.Players)
                {
                    await CreatureCmd.Damage(choiceContext, player.Creature, 10m, ValueProp.Unpowered, base.Owner);
                }
            }
        }

        public override bool ShouldDie(Creature creature)
        {
            if(creature != base.Owner)
                return base.ShouldDie(creature);

            if((int)MiyabiModConfig.CombatHardSelected >= 10)
            {
                return false;
            }
            return base.ShouldDie(creature);
        }

        public async override Task AfterPreventingDeath(Creature creature)
        {
            await CreatureCmd.Heal(creature, creature.MaxHp / 2);
        }
        
    }


    [HarmonyPatch(typeof(Hook), "BeforeCombatStart")]
    internal static class ModPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Task __result, CombatState combatState)
        {

            // 给所有玩家上 Buff
            foreach (var player in combatState.Players)
            {
                if (player?.Creature == null) continue;
                PowerCmd.Apply<ModConfigEffect>(new ThrowingPlayerChoiceContext(), player.Creature, 1, null, null);
            }

            // 给所有敌人上 Buff
            foreach (var enemy in combatState.Enemies)
            {
                if (enemy == null) continue;
                PowerCmd.Apply<ModConfigEnemyEffect>(new ThrowingPlayerChoiceContext(), enemy, 1, null, null);
            }
        }
    }

    [HarmonyPatch(typeof(Hook), "AfterActEntered")]
    internal static class ActPatch
    {
        private static async Task Postfix(Task __result, IRunState runState)
        {
            await __result;

            if (runState?.Players == null) return;

            foreach (var player in runState.Players)
            {
                if ((int)MiyabiModConfig.CombatHardSelected >= 9 && (player.Character is Miyabi || MiyabiModConfig.ChangeToAllPlayers))
                {
                    CardCreationOptions options = new CardCreationOptions([ModelDb.CardPool<CurseCardPool>()], CardCreationSource.Other, CardRarityOddsType.Uniform, (CardModel c) => c.Rarity == CardRarity.Curse);
                    List<CardModel> list = (from r in CardFactory.CreateForReward(player, 1, options)
                                            select r.Card).ToList();
                    if (list.Count > 0)
                    {
                        CardModel card = list[0];
                        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
                    }
                }
            }

            if (runState.Act.ActNumber() != 1) return;

            foreach (var player in runState.Players)
            {
                if ((int)MiyabiModConfig.CombatHardSelected >= 2 && (player.Character is Miyabi || MiyabiModConfig.ChangeToAllPlayers))
                {
                    player.Gold /= 2;
                }
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
                decimal hpPer = 1m;
                if ((int)MiyabiModConfig.CombatHardSelected >= 1)
                {
                    hpPer = 1.1m;
                }
                if ((int)MiyabiModConfig.CombatHardSelected >= 5)
                {
                    hpPer = 1.2m;
                }
                if ((int)MiyabiModConfig.CombatHardSelected >= 8)
                {
                    hpPer = 1.3m;
                }

                int num = (int)Math.Ceiling(__result.MaxHp * hpPer);
                //int num2 = __result.MaxHp + num;
                __result.SetMaxHpInternal(num);
                __result.SetCurrentHpInternal(num);


                decimal strength = 0;
                if((int)MiyabiModConfig.CombatHardSelected >= 4)
                {
                    strength = 1;
                }
                if ((int)MiyabiModConfig.CombatHardSelected >= 8)
                {
                    strength = 2;
                }
                PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), __result, strength, null, null);


                if ((int)MiyabiModConfig.CombatHardSelected >= 5)
                {
                    CreatureCmd.GainBlock(__result, 10m, ValueProp.Unpowered, null);
                }
            }
        }
    }

}
