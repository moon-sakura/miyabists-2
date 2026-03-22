using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        public static bool FrostFireKeeped { get; set; } = false;

        // 获取当前状态（可选，直接访问属性也行）
        public static bool ShouldKeepFrostFire() => FrostFireKeeped;

        // 设置状态
        public static void SetShouldKeepFrostFire(bool value) => FrostFireKeeped = value;

        //落霜消耗计数
        public static int LuoShuangCurrent { get; set; } = 0;
        public static int LuoShuangCostThisTurn { get; set; } = 0;
        public static void ResetLuoShuangCost() => LuoShuangCostThisTurn = 0;
        public static bool AddLuoShuangCost(int amount)
        {
            if (!CanUseLuoShuang(amount)) return false;
            LuoShuangCostThisTurn += amount;
            LuoShuangCurrent -= amount;
            return true;
        }
        public static int GetLuoShuangCost() => LuoShuangCostThisTurn;
        public static bool CanUseLuoShuang(int cost)
        {
            return cost <= LuoShuangCurrent;
        }
        public static async Task AddLuoShuang(int amount,Creature creature)
        {
            LuoShuangCurrent += amount;
            await PowerCmd.Apply<FrostFallPower>(creature, amount, null, null);
        }
    }
}
