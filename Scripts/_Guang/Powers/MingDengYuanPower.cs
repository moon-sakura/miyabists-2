using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts._Guang.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 明灯愿 - 能力。
    /// 被移除时，将一张 0 费的《斩妄开天》加入手卡。
    /// </summary>
    internal class MingDengYuanPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/mingDengYuan.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"被移除"
            if (!CombatManager.Instance.IsInProgress) return;
            if (oldOwner?.Player == null) return;
            if (oldOwner.CombatState == null) return;

            CardModel? card = oldOwner.CombatState.CreateCard<ZhanWangKaiTian>(oldOwner.Player);
            if (card == null) return;

            // 《斩妄开天》原为 2 费，加入手卡时变为 0 费
            card.EnergyCost.SetUntilPlayed(0, false);

            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, oldOwner.Player);
        }
    }
}
