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
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 明心镜状态。
    /// 进入时：消耗所有正面能力（支援点数、青溟剑势除外），每消耗 1 种为所有敌人附加 10% 失衡易伤，
    ///         扣除 10% 生命上限，并把战斗中所有 I2MingxinJing 的卡变为对应的明心镜卡。
    /// 期间：抽到的原卡也会自动变为明心镜卡。
    /// 移除时：所有明心镜卡变回原卡（保留升级状态）。
    /// </summary>
    internal class MingxinJingPower : ModPowerTemplate
    {
        /// <summary>每消耗 1 种正面能力，给所有敌人附加的失衡易伤（%）</summary>
        private const decimal VulnerablePerPower = 10m;

        /// <summary>进入状态时扣除的生命上限（%）</summary>
        private const decimal HpLossPercent = 10m;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/mingxinJing.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();

            Player owner = Owner.Player;
            if (owner == null) return;

            // 1. 消耗所有正面能力（支援点数与青溟剑势除外）
            List<PowerModel> consumed = Owner.Powers
                .Where(p => p != this
                    && p.Type == PowerType.Buff
                    && p is not SupportPointPower
                    && p is not QingmingJianshiPower)
                .ToList();

            foreach (PowerModel power in consumed)
            {
                await PowerCmd.Remove(power);
            }

            // 2. 每消耗 1 种正面能力，为所有敌人附加 10% 失衡易伤
            if (consumed.Count > 0)
            {
                decimal vulnerable = consumed.Count * VulnerablePerPower;
                foreach (Creature enemy in Owner.CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<DazeVulnPower>(choiceContext, enemy, vulnerable, Owner, null);
                }
            }

            // 3. 扣除 10% 生命上限
            await CreatureCmd.LoseMaxHp(choiceContext, Owner, Owner.MaxHp * HpLossPercent / 100m, false);

            // 4. 所有原卡进入明心镜形态
            await MingxinJingTransform.TransformAllInCombat(owner);
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"退出明心镜状态"，不处理变回原卡
            if (!CombatManager.Instance.IsInProgress) return;

            // 退出明心镜状态：所有明心镜卡变回原卡（保留升级状态）
            if (oldOwner?.Player == null) return;
            await MingxinJingTransform.RevertAllInCombat(oldOwner.Player);
        }

        public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
        /// <summary>明心镜状态下抽到的原卡也立刻变为明心镜卡</summary>
        {
            if (card == null || card.Owner != Owner.Player) return;
            if (!MingxinJingTransform.CanTransform(card)) return;
            await MingxinJingTransform.Transform(card);
        }

        /// <summary>
        /// 把《明心镜·归尘》加入手卡（由青溟剑势归 0 时调用，见 QingmingJianshiPower）。
        /// 手卡里已经有就不会重复加入。
        /// </summary>
        public async Task AddGuiChenIfNeeded()
        {
            Player owner = Owner?.Player;
            if (owner == null) return;
            if (Owner.CombatState == null) return;

            if (PileType.Hand.GetPile(owner).Cards.Any(c => c is MingxinGuiChen)) return;

            CardModel guiChen = Owner.CombatState.CreateCard<MingxinGuiChen>(owner);
            if (guiChen == null) return;

            await CardPileCmd.AddGeneratedCardToCombat(guiChen, PileType.Hand, owner);
        }
    }
}
