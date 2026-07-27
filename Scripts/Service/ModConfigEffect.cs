using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    [RegisterRelic(typeof(EventRelicPool))]
    internal class MiyabiModRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.None;
        public override string PackedIconPath => "res://images/relics/QinShiSpec.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        private int _counter;

        // 显示在遗物图标上的数字
        public override bool ShowCounter => true;
        public override int DisplayAmount => Counter;

        [SavedProperty]
        public int Counter
        {
            get => _counter;
            private set
            {
                AssertMutable();
                _counter = value;
                InvokeDisplayAmountChanged();
            }
        }
        public void SetCounter(int counter) => Counter = counter;


        public override async Task BeforeCombatStart()
        {
            if(Counter == 0)
            {
                return;
            }

            // 给所有玩家上 Buff
            foreach (var player in base.Owner.Creature.CombatState.Players)
            {
                if (player?.Creature == null || player.Creature.HasPower<ModConfigEffect>()) continue;
                (await PowerCmd.Apply<ModConfigEffect>(new ThrowingPlayerChoiceContext(), player.Creature, 1, null, null))?.SetHard(Counter);
            }

            // 给所有敌人上 Buff
            foreach (var enemy in base.Owner.Creature.CombatState.Enemies)
            {
                if (enemy == null || enemy.HasPower<ModConfigEnemyEffect>()) continue;
                (await PowerCmd.Apply<ModConfigEnemyEffect>(new ThrowingPlayerChoiceContext(), enemy, 1, null, null))?.SetHard(Counter);
            }
        }

        public override async Task AfterActEntered()
        {
            if ((int)MiyabiModConfig.CombatHardSelected >= 9 && (MiyabiFuncBase.IsMiyabiModChar(Owner) || MiyabiModConfig.ChangeToAllPlayers))
            {
                CardCreationOptions options = new CardCreationOptions([ModelDb.CardPool<CurseCardPool>()], CardCreationSource.Other, CardRarityOddsType.Uniform, (CardModel c) => c.Rarity == CardRarity.Curse);
                List<CardModel> list = (from r in CardFactory.CreateForReward(Owner, 1, options)
                                        select r.Card).ToList();
                if (list.Count > 0)
                {
                    CardModel card = list[0];
                    CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
                }
            }

            if (base.Owner.RunState.Act.ActNumber() != 1)
            {
                if ((int)MiyabiModConfig.CombatHardSelected >= 2 && (MiyabiFuncBase.IsMiyabiModChar(Owner) || MiyabiModConfig.ChangeToAllPlayers))
                {
                    CardModel card = Owner.Deck.Cards.Where(c => c.IsUpgraded).TakeRandom(1, Owner.RunState.Rng.Shuffle).FirstOrDefault();
                    if (card != null)
                        CardCmd.Downgrade(card);
                    GD.Print($"[MiyabiMod] : 已降级{card.ToString()}");
                }
            }
                

            if (Owner.RunState.Act.ActNumber() != 1) return;

            if (MiyabiFuncBase.IsMiyabiModChar(Owner))
            {
                if (Owner.Character is Miyabi && MiyabiModConfig.MiyabiFunPileSelected == MiyabiFunPile.BeeGroup)
                {
                    CardModel card = Owner.RunState.CreateCard<BeeGroup>(Owner);
                    card.UpgradeInternal();
                    CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
                }

                if (Owner.Character is Yixuan && MiyabiModConfig.YixuanFunPileSelected == YixuanFunPile.BeeGroup)
                {
                    CardModel card = Owner.RunState.CreateCard<BeeGroupYixuan>(Owner);
                    card.UpgradeInternal();
                    CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
                }

                if (Owner.Character is Miyabi && MiyabiModConfig.MiyabiFunPileSelected == MiyabiFunPile.Lucia)
                {
                    await RelicCmd.Obtain<TonghuaJishibenRelic>(Owner);
                }
                if (Owner.Character is Yixuan && MiyabiModConfig.YixuanFunPileSelected == YixuanFunPile.Lucia)
                {
                    await RelicCmd.Obtain<TonghuaJishibenRelic>(Owner);
                }
            }
        }
    }
    internal class ModConfigEffect : ModPowerTemplate
    {
        public override PowerType Type => PowerType.None;
        public override PowerStackType StackType => PowerStackType.Single;

        protected override bool IsVisibleInternal => false;

        private int playcount = 0;

        public int hard = 0;
        public void SetHard(int t) { hard = t; }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if((int)MiyabiModConfig.CombatHardSelected >= 4 && (MiyabiFuncBase.IsMiyabiModChar(Owner.Player) || MiyabiModConfig.ChangeToAllPlayers))
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -1, null, null);
            }
        }

        public async override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
        {
            if (!creature.IsEnemy) return;

            if ((int)MiyabiModConfig.CombatHardSelected >= 7 && (MiyabiFuncBase.IsMiyabiModChar(Owner.Player) || MiyabiModConfig.ChangeToAllPlayers))
            {
                //foreach (var player in creature.CombatState.Players)
                {
                    await CreatureCmd.Damage(choiceContext, Owner, 6m, ValueProp.Unpowered, base.Owner);
                }
            }
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;

            if (card.Owner.Creature != base.Owner) return false;

            if (!MiyabiFuncBase.IsMiyabiModChar(Owner.Player) && !MiyabiModConfig.ChangeToAllPlayers)
                return false;

            if((int)MiyabiModConfig.CombatHardSelected < 3)
                return false;

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

        public async override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner.Creature != base.Owner) return;

            playcount++;
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if((int)MiyabiModConfig.CombatHardSelected >= 6 && (MiyabiFuncBase.IsMiyabiModChar(Owner.Player) || MiyabiModConfig.ChangeToAllPlayers))
            {
                if(player.Creature.CombatState.RoundNumber <= 3)
                    return base.ModifyHandDraw(player, count) - 1;
            }
            return base.ModifyHandDraw(player, count);
        }
    }

    internal class ModConfigEnemyEffect : ModPowerTemplate
    {
        public override PowerType Type => PowerType.None;
        public override PowerStackType StackType => PowerStackType.Single;

        protected override bool IsVisibleInternal => false;

        private bool isRevealed = false;

        public int hard = 0;
        public void SetHard(int t) { hard = t; }

        //public async override Task BeforeDeath(Creature creature)
        //{
        //    if(creature != base.Owner) return;

        //    if((int)MiyabiModConfig.CombatHardSelected >= 6 && (creature.CombatState.Players.Any(c => c.MiyabiFuncBase.IsMiyabiModChar(c)) || MiyabiModConfig.ChangeToAllPlayers))
        //    {
        //        foreach(var player in creature.CombatState.Players)
        //        {
        //            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), player.Creature, 10m, ValueProp.Unpowered, base.Owner);
        //        }
        //    }
        //}

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            decimal hpPer = 1m;
            if (hard >= 1)
            {
                hpPer = 1.1m;
            }
            if (hard >= 5)
            {
                hpPer = 1.2m;
            }
            if (hard >= 8)
            {
                hpPer = 1.3m;
            }

            int num = (int)Math.Ceiling(Owner.MaxHp * hpPer);
            //int num2 = __result.MaxHp + num;
            Owner.SetMaxHpInternal(num);
            Owner.SetCurrentHpInternal(num);


            decimal strength = 0;
            if ((int)MiyabiModConfig.CombatHardSelected >= 4)
            {
                strength = 1;
            }
            if ((int)MiyabiModConfig.CombatHardSelected >= 8)
            {
                strength = 2;
            }
            PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, strength, null, null);

            return base.AfterApplied(applier, cardSource);
        }

        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner || amount < target.CurrentHp) return;

            if (!isRevealed && (int)MiyabiModConfig.CombatHardSelected >= 10 && (base.Owner.CombatState.Players.Any(c => MiyabiFuncBase.IsMiyabiModChar(c)) || MiyabiModConfig.ChangeToAllPlayers))
            {
                decimal heal = Math.Max(1m, base.Owner.MaxHp*0.3m);
                await CreatureCmd.Heal(base.Owner, heal);
                await PowerCmd.Apply<BufferPower>(choiceContext, base.Owner, 1m, base.Owner, null);
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 2m, base.Owner, null);
                isRevealed = true;
            }
        }

    }


    [HarmonyPatch(typeof(Hook), "AfterActEntered")]
    internal static class ActPatch
    {
        private static bool Prefix(IRunState runState)
        {
            int max = 0;
            if (runState?.Players != null)
            {
                foreach (var player in runState.Players)
                {
                    RelicCmd.Obtain<MiyabiModRelic>(player);
                    ((MiyabiModRelic)MiyabiFuncBase.GetRelic<MiyabiModRelic>(player)).SetCounter((int)MiyabiModConfig.CombatHardSelected);
                }

                foreach (var player in runState.Players)
                {
                    int c = ((MiyabiModRelic)MiyabiFuncBase.GetRelic<MiyabiModRelic>(player)).Counter;
                    if (c > max)
                        max = c;
                }

                foreach (var player in runState.Players)
                {
                    ((MiyabiModRelic)MiyabiFuncBase.GetRelic<MiyabiModRelic>(player)).SetCounter(max);
                }
            }
            return true;
        }

        //private static async Task Postfix(Task __result, IRunState runState)
        //{
        //    await __result;
        //
        //    if (runState?.Players == null) return;
        //
        //    foreach (var player in runState.Players)
        //    {
        //        if ((int)MiyabiModConfig.CombatHardSelected >= 9 && (MiyabiFuncBase.IsMiyabiModChar(player) || MiyabiModConfig.ChangeToAllPlayers))
        //        {
        //            CardCreationOptions options = new CardCreationOptions([ModelDb.CardPool<CurseCardPool>()], CardCreationSource.Other, CardRarityOddsType.Uniform, (CardModel c) => c.Rarity == CardRarity.Curse);
        //            List<CardModel> list = (from r in CardFactory.CreateForReward(player, 1, options)
        //                                    select r.Card).ToList();
        //            if (list.Count > 0)
        //            {
        //                CardModel card = list[0];
        //                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        //            }
        //        }
        //
        //        if (runState.Act.ActNumber() == 1)
        //            continue;
        //        if ((int)MiyabiModConfig.CombatHardSelected >= 2 && (MiyabiFuncBase.IsMiyabiModChar(player) || MiyabiModConfig.ChangeToAllPlayers))
        //        {
        //            CardModel card = player.Deck.Cards.Where(c => c.IsUpgraded).TakeRandom(1,player.RunState.Rng.Shuffle).FirstOrDefault();
        //            if(card != null)
        //                CardCmd.Downgrade(card);
        //            GD.Print($"[MiyabiMod] : 已降级{card.ToString()}");
        //        }
        //    }
        //
        //    if (runState.Act.ActNumber() != 1) return;
        //
        //    foreach (var player in runState.Players)
        //    {
        //        //if ((int)MiyabiModConfig.CombatHardSelected >= 2 && (player.MiyabiFuncBase.IsMiyabiModChar(c) || MiyabiModConfig.ChangeToAllPlayers))
        //        //{
        //        //    player.Gold /= 2;
        //        //}
        //
        //        if(MiyabiFuncBase.IsMiyabiModChar(player))
        //        {
        //            if(player.Character is Miyabi && MiyabiModConfig.MiyabiFunPileSelected == MiyabiFunPile.BeeGroup)
        //            {
        //                CardModel card = player.RunState.CreateCard<BeeGroup>(player);
        //                card.UpgradeInternal();
        //                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        //            }
        //
        //            if(player.Character is Yixuan && MiyabiModConfig.YixuanFunPileSelected == YixuanFunPile.BeeGroup)
        //            {
        //                CardModel card = player.RunState.CreateCard<BeeGroupYixuan>(player);
        //                card.UpgradeInternal();
        //                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        //            }
        //
        //            if(player.Character is Miyabi && MiyabiModConfig.MiyabiFunPileSelected == MiyabiFunPile.Lucia)
        //            {
        //                await RelicCmd.Obtain<TonghuaJishibenRelic>(player);
        //            }
        //            if (player.Character is Yixuan && MiyabiModConfig.YixuanFunPileSelected == YixuanFunPile.Lucia)
        //            {
        //                await RelicCmd.Obtain<TonghuaJishibenRelic>(player);
        //            }
        //        }
        //    }
        //}
    }



    //[HarmonyPatch(typeof(Hook), "BeforeCombatStart")]
    //internal static class ModPatch
    //{
    //    [HarmonyPostfix]
    //    private static void Postfix(Task __result, CombatState combatState)
    //    {

    //        // 给所有玩家上 Buff
    //        foreach (var player in combatState.Players)
    //        {
    //            if (player?.Creature == null) continue;
    //            PowerCmd.Apply<ModConfigEffect>(new ThrowingPlayerChoiceContext(), player.Creature, 1, null, null);
    //        }

    //        // 给所有敌人上 Buff
    //        foreach (var enemy in combatState.Enemies)
    //        {
    //            if (enemy == null) continue;
    //            PowerCmd.Apply<ModConfigEnemyEffect>(new ThrowingPlayerChoiceContext(), enemy, 1, null, null);
    //        }
    //    }
    //}



    //[HarmonyPatch(typeof(CombatState), "CreateCreature")]
    //internal static class MonsterMaxHpPatch
    //{
    //    private static void Postfix(Creature __result, CombatSide side)
    //    {
    //        if (side == CombatSide.Enemy && (__result.CombatState.Players.Any(c => MiyabiFuncBase.IsMiyabiModChar(c)) || MiyabiModConfig.ChangeToAllPlayers))
    //        {
    //            decimal hpPer = 1m;
    //            if ((int)MiyabiModConfig.CombatHardSelected >= 1)
    //            {
    //                hpPer = 1.1m;
    //            }
    //            if ((int)MiyabiModConfig.CombatHardSelected >= 5)
    //            {
    //                hpPer = 1.2m;
    //            }
    //            if ((int)MiyabiModConfig.CombatHardSelected >= 8)
    //            {
    //                hpPer = 1.3m;
    //            }

    //            int num = (int)Math.Ceiling(__result.MaxHp * hpPer);
    //            //int num2 = __result.MaxHp + num;
    //            __result.SetMaxHpInternal(num);
    //            __result.SetCurrentHpInternal(num);


    //            decimal strength = 0;
    //            if((int)MiyabiModConfig.CombatHardSelected >= 4)
    //            {
    //                strength = 1;
    //            }
    //            if ((int)MiyabiModConfig.CombatHardSelected >= 8)
    //            {
    //                strength = 2;
    //            }
    //            PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), __result, strength, null, null);


    //            //if ((int)MiyabiModConfig.CombatHardSelected >= 5)
    //            //{
    //            //    CreatureCmd.GainBlock(__result, 10m, ValueProp.Unpowered, null);
    //            //}
    //        }
    //    }
    //}

}
