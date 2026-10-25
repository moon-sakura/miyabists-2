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
    ///         并把战斗中所有 I2MingxinJing 的卡变为对应的明心镜卡。
    /// 期间：抽到的原卡也会自动变为明心镜卡；《明心镜·归尘》不在手卡时每次出牌后自动补回手卡。
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

        /// <summary>
        /// 进入 / 退出明心镜状态时会批量变牌，变牌过程中会触发 AfterCardChangedPiles，
        /// 用这个标记避免把刚变回来的原卡又变回去。
        /// </summary>
        private bool _suppressGuiChenHook;

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

            // 3. 扣除 10% 生命上限（暂时关闭，测试后再决定是否加回来）
            // await CreatureCmd.LoseMaxHp(choiceContext, Owner, Owner.MaxHp * HpLossPercent / 100m, false);

            // 4. 所有原卡进入明心镜形态
            _suppressGuiChenHook = true;
            try
            {
                await MingxinJingTransform.TransformAllInCombat(owner);
            }
            finally
            {
                _suppressGuiChenHook = false;
            }
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"退出明心镜状态"，不处理变回原卡
            if (!CombatManager.Instance.IsInProgress) return;

            // 退出明心镜状态：所有明心镜卡变回原卡（保留升级状态）
            if (oldOwner?.Player == null) return;

            _suppressGuiChenHook = true;
            try
            {
                await MingxinJingTransform.RevertAllInCombat(oldOwner.Player);
            }
            finally
            {
                _suppressGuiChenHook = false;
            }
        }

        public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
        /// <summary>明心镜状态下抽到的原卡也立刻变为明心镜卡</summary>
        {
            if (card == null || card.Owner != Owner.Player) return;
            if (!MingxinJingTransform.CanTransform(card)) return;
            await MingxinJingTransform.Transform(card);
        }

        /// <summary>
        /// 打出照影后它会进入弃牌堆，这时把它变成《明心镜·归尘》（变牌时会自动放进手卡）。
        /// 照影在结算过程中还停在 Play 堆里，所以要等它真正进堆之后再变。
        /// </summary>
        public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel clonedBy)
        {
            if (_suppressGuiChenHook) return;
            if (card == null || card.Owner != Owner?.Player) return;
            if (card.Pile == null || card.Pile.Type == PileType.Play) return;
            if (card is not ZhaoYing) return;

            _suppressGuiChenHook = true;
            try
            {
                await MingxinJingTransform.Transform(card);
            }
            finally
            {
                _suppressGuiChenHook = false;
            }
        }

        /// <summary>每次出牌后，如果《明心镜·归尘》不在手卡就补回手卡</summary>
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (_suppressGuiChenHook) return;
            if (cardPlay?.Card?.Owner != Owner?.Player) return;
            await AddGuiChenIfNeeded();
        }

        /// <summary>
        /// 确保《明心镜·归尘》在手卡里：
        /// 已经在手卡就不动；在抽牌堆 / 弃牌堆 / 消耗堆就挪进手卡；
        /// 还没有的话，把还在原卡形态的照影变过去（变牌时自动放入手卡）。
        /// 手卡没位置时放到抽卡堆顶部。
        /// </summary>
        public async Task AddGuiChenIfNeeded()
        {
            Player? owner = Owner?.Player;
            if (owner?.PlayerCombatState == null) return;

            CardPile hand = PileType.Hand.GetPile(owner);
            if (hand.Cards.Any(c => c is MingxinGuiChen)) return;

            // 先快照再处理，避免挪动卡牌时改动牌堆导致遍历失效
            List<CardModel> cardsInCombatPiles = owner.PlayerCombatState.AllPiles
                .Where(p => p.Type != PileType.Play)
                .SelectMany(p => p.Cards)
                .ToList();

            CardModel? existing = cardsInCombatPiles.FirstOrDefault(c => c is MingxinGuiChen);
            if (existing != null)
            {
                await MingxinJingTransform.PlaceGuiChenInHand(owner, existing);
                return;
            }

            CardModel? zhaoYing = cardsInCombatPiles.FirstOrDefault(c => c is ZhaoYing);
            if (zhaoYing == null) return;

            _suppressGuiChenHook = true;
            try
            {
                await MingxinJingTransform.Transform(zhaoYing);
            }
            finally
            {
                _suppressGuiChenHook = false;
            }
        }
    }
}
