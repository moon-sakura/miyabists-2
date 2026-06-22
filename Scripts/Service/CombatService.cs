using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using System.Drawing;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        //伙伴卡牌的特殊处理
        //public static bool ThisTurnUsedPartnerCard { set; get; } = false;
        //public static bool GetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard;
        //public static void ResetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard = false;
        //public static void UsedPartnerCard() => ThisTurnUsedPartnerCard = true;
        public static void SetSupportCostToZero(PlayerChoiceContext choiceContext, CardModel card, Player owner)
        {
            if(owner.Character is Miyabi)
            {
                
            }

            if (owner.Character is Yixuan)
            {

            }
        }


        public static bool AnoNeedCheck { get; set; } = true;
        public static bool DazeNeedCheck { get; set; } = true;
        public static bool FrostNeedCheck { get; set; } = true;

        public static void ResetCheck()
        {
            AnoNeedCheck = true;
            DazeNeedCheck = true;
            FrostNeedCheck = true;
        }

        //属性积蓄判断
        private static int _anoTrigger = 5;
        public static int AnoTrigger { get; set; } = _anoTrigger;
        public static void ChangeAnoT(int amount) => AnoTrigger += amount;
        public static void ResetAnoT() => AnoTrigger = _anoTrigger;
        public static int GetAnoTrigger() => AnoTrigger;

        public static void SetAnoTriggerMultiply(Creature c)
        {

            int mul = c.CombatState.PlayerCreatures.Count;

            if (AnoNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeAnoT((mul - 1) * 2);
                    AnoNeedCheck = false;
                }
                else
                {
                    ResetAnoT();
                    AnoNeedCheck = false;
                }
            }
            
        }

        //失衡值判断
        private static int _dazeTrigger = 100;
        public static int DazeTrigger { get; set; } = _dazeTrigger;
        public static void ChangeDazeT(int amount) => DazeTrigger += amount;
        public static void ResetDazeT() => DazeTrigger = _dazeTrigger;
        public static int GetDazeTrigger() => DazeTrigger;

        public static void SetDazeTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;

            if (DazeNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeDazeT((mul - 1) * 20);
                    DazeNeedCheck = false;
                }
                else
                {
                    ResetDazeT();
                    DazeNeedCheck = false;
                }
            }
            
        }

        //烈霜值判断
        private static int _frostTrigger = 50;
        public static int FrostTrigger { get; set; } = _frostTrigger;
        public static void ChangeFrostT(int amount) => FrostTrigger += amount;
        public static void ResetFrostT() => FrostTrigger = _frostTrigger;
        public static int GetFrostTrigger() => FrostTrigger;

        public static bool CanAddFrostWhenFire { get; set; } = false;
        public static bool GetCanAddWhenFire() => CanAddFrostWhenFire;
        public static bool SetCanAddWhenFire(bool value) => CanAddFrostWhenFire = value;
        public static void ResetCanAddWhenFire() => CanAddFrostWhenFire = false;


        public static void SetFrostTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;
            if (FrostNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeFrostT((mul - 1) * 15);
                    FrostNeedCheck = false;
                }
                else
                {
                    ResetFrostT();
                    FrostNeedCheck = false;
                }
            }
        }

        //新月祝福
        public static bool IsAnyHasMoonBlessing(Creature c)
        {
            foreach (Creature Player in c.CombatState.PlayerCreatures)
            {
                if (Player != null && Player.IsAlive && Player.HasPower<BlessingMoonPower>())
                {
                    return true;
                }
                //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
            return false;
        }

        //烈霜
        public static decimal FrostFireLimit { get; set; } = 0.5m;
        public static decimal GetFrostFireLimit() => FrostFireLimit;
        public static decimal SetFrostFireLimit(decimal value) => FrostFireLimit = value;
        public static void ResetFrostFireLimit() => FrostFireLimit = 0.5m;

        public static async Task AddAnoBuildup(Creature target, int anoVar, Creature dealer, CardModel card, PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            SetAnoTriggerMultiply(target);

            //处理Amount而不是DisplayAmount
            int trigger = GetAnoTrigger() + 1;
            bool hasAnomaly = target.HasPower<AttributeAnomalyPower>();
            int chkAno = anoVar;
            if (target.HasPower<AnomalyBuildupPower>())
                chkAno += target.GetPowerAmount<AnomalyBuildupPower>();


            // 情况 A：已经有异常状态了
            if (hasAnomaly)
            {
                if (chkAno >= trigger && chkAno < 2 * trigger) // 满溢则紊乱
                {
                    await DisorderApply(target, dealer, choiceContext);
                    //await PowerCmd.SetAmount<AnomalyBuildupPower>(target, chkAno - trigger + 1, dealer, card);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<AnomalyBuildupPower>(), chkAno - trigger + 1, dealer, null);
                }
                else if(chkAno >= 2 * trigger)
                {
                    await DisorderApply(target, dealer, choiceContext);
                    await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, dealer, card);
                    //await PowerCmd.SetAmount<AnomalyBuildupPower>(target, chkAno - 2*trigger + 1, dealer, card);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<AnomalyBuildupPower>(), chkAno - 2 * trigger + 1, dealer, null);
                }
                else // 未满则继续堆积蓄
                {
                    await PowerCmd.Apply<AnomalyBuildupPower>(choiceContext, target, anoVar, dealer, card);
                }
            }
            // 情况 B：还没有异常状态
            else
            {
                if (chkAno >= trigger && chkAno < 2 * trigger) // 满溢则触发异常
                {
                    await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, dealer, card);
                    //await PowerCmd.Apply<AnomalyBuildupPower>(target, 1-trigger, dealer, card);
                    //await PowerCmd.SetAmount<AnomalyBuildupPower>(choiceContext, target, chkAno - trigger + 1, dealer, card);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<AnomalyBuildupPower>(), chkAno - trigger + 1, dealer, null);

                }
                else if(chkAno >= 2 * trigger)
                {
                    await DisorderApply(target, dealer, choiceContext);
                    //await PowerCmd.SetAmount<AnomalyBuildupPower>(target, chkAno - 2*trigger + 1, dealer, card);
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<AnomalyBuildupPower>(), chkAno - 2 * trigger + 1, dealer, null);
                }
                else // 未满则仅仅添加积蓄
                {
                    await PowerCmd.Apply<AnomalyBuildupPower>(choiceContext, target, anoVar, dealer, card);
                }
            }
        }

        public static decimal DisorderDamageRate { get; set; } = 0.1m;
        public static void ResetDisorderDamageRate() { DisorderDamageRate = 0.1m; }

        //用于调用各种触发
        //紊乱触发
        public static async Task DisorderApply(Creature target, Creature dealer, PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            bool moonBless = false;
            if (dealer.IsPlayer)
            {
                moonBless = IsAnyHasMoonBlessing(dealer);
            }
            bool hasZmyc = target.HasPower<ZhongmuycPower>();

            await PowerCmd.Remove<AttributeAnomalyPower>(target);

            //造成10%点伤害
            decimal damage = target.MaxHp * DisorderDamageRate;

            if (moonBless) damage += target.MaxHp * 0.05m;

            if (hasZmyc) damage *= 1.5m;

            await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Unpowered | ValueProp.Unblockable, dealer);

            await PowerCmd.Apply<DisorderPower>(choiceContext, target, 1, dealer, null);
        }

        //霜灼增加
        public static async Task FrostApply(Creature target, Creature dealer , PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            bool hasHanyan = dealer.Player.PlayerCombatState.AllCards.Any(c => c is ShenxueHanyan);

            SetFrostTriggerMultiply(target);

            await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<FrostBuildPower>(), 1, dealer, null);
            
            if(hasHanyan)
            {
                await PowerCmd.Apply<FrostpoPower>(choiceContext, target, 1, dealer, null);
                int fireAmount = target.GetPowerAmount<FrostFirePower>();

                await CreatureCmd.Damage(choiceContext, target, fireAmount * 5m, ValueProp.Unpowered, dealer);
                if(fireAmount / 5m >= 1)
                    await PowerCmd.Apply<StrengthPower>(choiceContext, dealer, fireAmount / 5m, dealer, null);

                await PowerCmd.Remove(target.GetPower<FrostFirePower>());
            }
            else
            {
                await PowerCmd.Apply<FrostPower>(choiceContext, target, 1, dealer, null);
            }



            if (target.HasPower<AttributeAnomalyPower>())
            {
                await DisorderApply(target, dealer, choiceContext);
            }
            else
            {
                await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, dealer, null);
            }
        }

        //失衡值叠加
        public static async Task AddDaze(PlayerChoiceContext choiceContext, Creature target,DynamicVar dazeVar,Creature dealer)
        {
            if (target == null || target.IsDead) return;

            SetDazeTriggerMultiply(target);

            int chkDaze = target.GetPowerAmount<DazePower>() + dazeVar.IntValue;

            if (!target.HasPower<BreakPower>() && chkDaze <= DazeTrigger)
                await PowerCmd.Apply<DazePower>(choiceContext, target, dazeVar.BaseValue, dealer, null);
            else if (chkDaze >= DazeTrigger + 1)
            {
                //await PowerCmd.SetAmount<DazePower>(target, 1, dealer, null);
                await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<DazePower>(), 1, dealer, null);
                await PowerCmd.Apply<BreakPower>(choiceContext, target, 1, dealer, null);
            }
        }

        public static async Task DazeAddtoPlayer(PlayerChoiceContext choiceContext, Creature player, int count)
        {
            if (player == null || player.IsDead) return;

            int chkDaze = count;
            if (player.HasPower<DazePower>())
            {
                chkDaze += player.GetPowerAmount<DazePower>();
            }
            
            if(chkDaze >= 100)
            {
                await PowerCmd.Apply<BreakPlayerPower>(choiceContext, player, 1, null, null);
                await PowerCmd.Remove<DazePower>(player);
            }
            else if(!player.HasPower<BreakPlayerPower>())
            {
                await PowerCmd.Apply<DazePower>(choiceContext, player, count, null, null);
            }

        }

        //加喧嚣值
        public static async Task AddDecible(Player player, int amount)
        {
            // 查找所有实现IDecibleCounter接口的遗物（包括Miyabi/Yixuan/Guang的喧嚣值遗物）
            var decibleRelics = player.Relics.OfType<IDecibleCounter>();

            foreach (var relic in decibleRelics)
            {
                relic.AddCounter(amount);
            }
        }

        //花辞
        public static async Task AddHuaCiReward(Creature owner, Creature target, PlayerChoiceContext choiceContext, int Count)
        {
            int handSize = owner.Player.PlayerCombatState.Hand.Cards.Count;
            if (target == null)
                target = owner.CombatState.HittableEnemies.TakeRandom(1, owner.Player.RunState.Rng.CombatCardSelection).FirstOrDefault();

            for (int i = 0; i < Count; i++)
            {
                CardModel reward1 = owner.CombatState.CreateCard<HuaCi>(owner.Player);

                if (handSize + i <= RitsuLibFramework.GetMaxHandSize(owner.Player))
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, owner.Player, CardPilePosition.Random);
                else
                    await CardCmd.AutoPlay(choiceContext, reward1, target);
            }
        }
    }
}
