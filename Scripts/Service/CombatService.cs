using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System.Drawing;
using MegaCrit.Sts2.Core.Logging;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        public static bool FrostFireKeeped { get; set; } = false;

        // 获取当前状态（可选，直接访问属性也行）
        public static bool ShouldKeepFrostFire() => FrostFireKeeped;

        // 设置状态
        public static void SetShouldKeepFrostFire(bool value) => FrostFireKeeped = value;

        //伙伴卡牌的特殊处理
        //public static bool ThisTurnUsedPartnerCard { set; get; } = false;
        //public static bool GetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard;
        //public static void ResetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard = false;
        //public static void UsedPartnerCard() => ThisTurnUsedPartnerCard = true;

        //属性积蓄判断
        public static int AnoTrigger { get; set; } = 5;
        public static void ChangeAnoT(int amount) => AnoTrigger += amount;
        public static void ResetAnoT() => AnoTrigger = 5;
        public static int GetAnoTrigger() => AnoTrigger;


        //用于调用各种触发
        //紊乱触发
        public static async Task DisorderApply(Creature target, Creature dealer, PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Remove<AttributeAnomalyPower>(target);
            await PowerCmd.Apply<DisorderPower>(target, 1, dealer, null);
            //造成20点伤害
            await CreatureCmd.Damage(choiceContext, target, 20, ValueProp.Unpowered, dealer);
        }

        //霜灼增加
        public static async Task FrostApply(Creature target, Creature dealer , PlayerChoiceContext choiceContext)
        {
            await CreatureCmd.Damage(choiceContext, target, 20, ValueProp.Unpowered, dealer);

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
            int chkDaze = target.GetPowerAmount<DazePower>() + dazeVar.IntValue;

            if (!target.HasPower<BreakPower>() && chkDaze <= 100)
                await PowerCmd.Apply<DazePower>(target, dazeVar.BaseValue, dealer, null);
            else if (chkDaze >= 101)
            {
                await PowerCmd.SetAmount<DazePower>(target, 1, dealer, null);
                await PowerCmd.Apply<BreakPower>(target, 1, dealer, null);
            }
        }


    }
}
