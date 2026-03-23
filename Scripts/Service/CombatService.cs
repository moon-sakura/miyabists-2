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
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System.Drawing;

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
        public static bool ThisTurnUsedPartnerCard { set; get; } = false;
        public static bool GetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard;
        public static void ResetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard = false;
        public static void UsedPartnerCard() => ThisTurnUsedPartnerCard = true;
    }
}
