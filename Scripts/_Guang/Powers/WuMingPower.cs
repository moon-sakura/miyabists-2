using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 无明 - 照破无明施加的层数计数。
    /// 被移除时：消耗所有诅咒卡，每消耗1张为所有敌人附加10%失衡易伤，
    ///           然后每层对所有敌人造成8点伤害（先加易伤再造成伤害）。
    /// </summary>
    internal class WuMingPower : ModPowerTemplate
    {
        /// <summary>每消耗 1 张诅咒卡，给所有敌人附加的失衡易伤（%）</summary>
        private const decimal VulnerablePerCurse = 10m;

        /// <summary>每层对敌人造成的伤害</summary>
        private const decimal DamagePerStack = 8m;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/wuMing.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"被移除"
            if (!CombatManager.Instance.IsInProgress) return;
            if (oldOwner?.Player == null) return;
            if (oldOwner.CombatState == null) return;

            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();

            // 1. 消耗所有诅咒卡（手卡 / 抽牌堆 / 弃牌堆）
            int cursedCount = 0;
            foreach (PileType pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
            {
                // 先快照再消耗，避免遍历过程中牌堆被改动
                List<CardModel> curses = pileType.GetPile(oldOwner.Player).Cards
                    .Where(c => c.Type == CardType.Curse)
                    .ToList();

                foreach (CardModel curse in curses)
                {
                    await CardCmd.Exhaust(choiceContext, curse);
                    cursedCount++;
                }
            }

            // 2. 每消耗 1 张诅咒卡，为所有敌人附加 10% 失衡易伤
            if (cursedCount > 0)
            {
                decimal vulnerable = cursedCount * VulnerablePerCurse;
                foreach (Creature enemy in oldOwner.CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<DazeVulnPower>(choiceContext, enemy, vulnerable, oldOwner, null);
                }
            }

            // 3. 每层对所有敌人造成 8 点伤害
            if (Amount <= 0) return;

            decimal damage = Amount * DamagePerStack;
            await CreatureCmd.Damage(choiceContext, oldOwner.CombatState.HittableEnemies,
                damage, ValueProp.Unpowered, oldOwner);
        }
    }
}
