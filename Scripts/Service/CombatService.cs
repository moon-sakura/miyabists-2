using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using System.Drawing;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        public static bool FrostFireKeeped { get; set; } = false;

        // 获取当前状态
        public static bool ShouldKeepFrostFire() => FrostFireKeeped;

        // 设置状态
        public static void SetShouldKeepFrostFire(bool value) => FrostFireKeeped = value;

        //伙伴卡牌的特殊处理
        //public static bool ThisTurnUsedPartnerCard { set; get; } = false;
        //public static bool GetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard;
        //public static void ResetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard = false;
        //public static void UsedPartnerCard() => ThisTurnUsedPartnerCard = true;

        //属性积蓄判断
        private static int _anoTrigger = 5;
        public static int AnoTrigger { get; set; } = _anoTrigger;
        public static void ChangeAnoT(int amount) => AnoTrigger += amount;
        public static void ResetAnoT() => AnoTrigger = _anoTrigger;
        public static int GetAnoTrigger() => AnoTrigger;

        public static void SetAnoTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;
            if(mul > 1)
            {
                ChangeAnoT((mul - 1) * 2);
            }
            else
            {
                ResetAnoT();
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
            if (mul > 1)
            {
                ChangeDazeT((mul - 1) * 20);
            }
            else
            {
                ResetDazeT();
            }
        }

        //烈霜值判断
        private static int _frostTrigger = 50;
        public static int FrostTrigger { get; set; } = _frostTrigger;
        public static void ChangeFrostT(int amount) => FrostTrigger += amount;
        public static void ResetFrostT() => FrostTrigger = _frostTrigger;
        public static int GetFrostTrigger() => FrostTrigger;

        public static void SetFrostTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;
            if (mul > 1)
            {
                ChangeFrostT((mul - 1) * 15);
            }
            else
            {
                ResetFrostT();
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

        //冰焰
        public static decimal FrostFireLimit { get; set; } = 0.5m;
        public static decimal GetFrostFireLimit() => FrostFireLimit;
        public static decimal SetFrostFireLimit(decimal value) => FrostFireLimit = value;
        public static void ResetFrostFireLimit() => FrostFireLimit = 0.5m;

        public static async Task AddAnoBuildup(Creature target, int anoVar, Creature dealer, CardModel card, PlayerChoiceContext choiceContext)
        {
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
                if (chkAno >= trigger) // 满溢则紊乱
                {
                    await DisorderApply(target, dealer, choiceContext);
                    await PowerCmd.SetAmount<AnomalyBuildupPower>(target, chkAno - trigger + 1, dealer, card);
                }
                else // 未满则继续堆积蓄
                {
                    await PowerCmd.Apply<AnomalyBuildupPower>(target, anoVar, dealer, card);
                }
            }
            // 情况 B：还没有异常状态
            else
            {
                if (chkAno >= trigger) // 满溢则触发异常
                {
                    await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, dealer, card);
                    //await PowerCmd.Apply<AnomalyBuildupPower>(target, 1-trigger, dealer, card);
                    await PowerCmd.SetAmount<AnomalyBuildupPower>(target, chkAno - trigger + 1, dealer, card);
                }
                else // 未满则仅仅添加积蓄
                {
                    await PowerCmd.Apply<AnomalyBuildupPower>(target, anoVar, dealer, card);
                }
            }
        }

        public static decimal DisorderDamageRate { get; set; } = 0.1m;
        public static void ResetDisorderDamageRate() { DisorderDamageRate = 0.1m; }

        //用于调用各种触发
        //紊乱触发
        public static async Task DisorderApply(Creature target, Creature dealer, PlayerChoiceContext choiceContext)
        {

            bool moonBless = IsAnyHasMoonBlessing(dealer);
            bool hasZmyc = target.HasPower<ZhongmuycPower>();

            await PowerCmd.Remove<AttributeAnomalyPower>(target);
            await PowerCmd.Apply<DisorderPower>(target, 1, dealer, null);
            //造成10%点伤害
            decimal damage = target.MaxHp * DisorderDamageRate;

            if (moonBless) damage += target.MaxHp * 0.05m;

            if (hasZmyc) damage *= 1.5m;

            await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Unpowered & ValueProp.Unblockable, dealer);
        }

        //霜灼增加
        public static async Task FrostApply(Creature target, Creature dealer , PlayerChoiceContext choiceContext)
        {
            SetFrostTriggerMultiply(target);
            //await CreatureCmd.Damage(choiceContext, target, 20, ValueProp.Unpowered, dealer);

            await PowerCmd.SetAmount<FrostBuildPower>(target, 1, dealer, null);
            await PowerCmd.Apply<FrostPower>(target, 1, dealer, null);


            int fireAmount = target.GetPowerAmount<Miyabists2.Scripts.Powers.FrostFirePower>();
            //Log.Info(">>> [MiyabiMod] 补丁:冰焰数值为： " + fireAmount ); // 建议加一行日志
            //if (fireAmount > 0)//target.HasPower<Miyabists2.Scripts.Powers.FrostFirePower>())
            //{
                //造成冰焰层数*1.5点伤害，清除冰焰
                //int fireAmount = target.GetPowerAmount<Miyabists2.Scripts.Powers.FrostFirePower>();

                //await CreatureCmd.Damage(null, base.Owner, fireAmount * 1.5m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, base.Owner);

            await CreatureCmd.Damage(choiceContext, target, 10m, ValueProp.Unpowered, dealer);

            if (!ShouldKeepFrostFire())
                await PowerCmd.Remove<FrostFirePower>(target);
            //}

            if (target.HasPower<AttributeAnomalyPower>())
            {
                await MiyabiCombatService.DisorderApply(target,dealer, choiceContext);
            }
            else
            {
                await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, dealer, null);
            }
        }

        //失衡值叠加
        public static async Task AddDaze(Creature target,DynamicVar dazeVar,Creature dealer)
        {
            SetDazeTriggerMultiply(target);

            int chkDaze = target.GetPowerAmount<DazePower>() + dazeVar.IntValue;

            if (!target.HasPower<BreakPower>() && chkDaze <= DazeTrigger)
                await PowerCmd.Apply<DazePower>(target, dazeVar.BaseValue, dealer, null);
            else if (chkDaze >= DazeTrigger + 1)
            {
                await PowerCmd.SetAmount<DazePower>(target, 1, dealer, null);
                await PowerCmd.Apply<BreakPower>(target, 1, dealer, null);
            }
        }

        //加喧嚣值
        public static async Task AddDecible(Player player, int amount)
        {
            // 查找实例
            var myRelic = player.Relics.OfType<SwordNotailRelic>().FirstOrDefault();
            var myRelic2 = player.Relics.OfType<NoTailFullRelic>().FirstOrDefault();

            // 修改属性
            if (myRelic != null)
            {
                myRelic?.AddCounter(amount);
            }

            if (myRelic2 != null)
            {
                myRelic2?.AddCounter(amount);
            }
        }
    }
}
