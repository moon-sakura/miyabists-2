using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Miyabists2.Scripts._Guang.Relics
{
    /// <summary>
    /// 青溟剑匣 - 叶瞬光初始遗物
    /// 不处于明心镜状态时，每获得 1 个自身没有的新能力，获得 1 点青溟剑势；
    /// 每使用 1 张卡牌计数 +1，计数达到 30 时将一张《逐云惊霆+》加入手卡。
    /// </summary>
    [RegisterRelic(typeof(YeshunGuangRelicPool))]
    [RegisterTouchOfOrobasRefinement(typeof(QingmingJianRelic))]
    internal class QingmingJianxiaRelic : ModRelicTemplate, IDecibleCounter
    {
        /// <summary>触发阈值（同时是计数上限）</summary>
        private const int DefaultThreshold = 30;

        public override RelicRarity Rarity => RelicRarity.Starter;

        // TODO: 替换为青溟剑匣专属遗物图标（images/_Guang/relics/qingmingJianxia.png）
        public override string PackedIconPath => "res://images/relics/swordNotail200.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
            HoverTipFactory.FromCard<ZhuYunJingTing>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill),
        ];

        public int Threshold { get; set; } = DefaultThreshold; // 触发阈值
        public int Max { get; set; } = DefaultThreshold;

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

        /// <summary>
        /// 本次能力变化之前是否已经拥有这个能力。
        /// BeforePowerAmountChanged 里记下来，AfterPowerAmountChanged 里用来判断「新能力」。
        /// </summary>
        private bool _hadPowerBefore = true;

        /// <summary>
        /// 手卡里是否已经有终结技（逐云惊霆 / 斩妄开天）。
        /// 有的话计数直接按一个阈值算：既不继续累加，也不会重复发放。
        /// 与 SwordNotailRelic / QingmingShufaRelic 里 hasEnd 的判断保持一致。
        /// </summary>
        private bool HasEndSkillInHand =>
            Owner?.PlayerCombatState?.Hand.Cards.Any(c => c.CanonicalKeywords.Contains(MiyabiKeywords.EndSkill) && c is ZhuYunJingTing) ?? false;

        public void AddCounter(int amount, bool forceAdd = false)
        {
            // 手卡已有终结技时计数直接加一个阈值（等价于把计数冻结在 Max）
            int counter = Counter + (HasEndSkillInHand ? Threshold : 0);

            if (counter < Max || forceAdd)
                this.Counter += amount;
        }

        public void SetMax(int amount) => Max = amount;
        public void ResetMax() => Max = DefaultThreshold;

        public int GetCounter() => Counter;

        // ==================== 获得新能力 -> 获得青溟剑势 ====================

        public override Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
        {
            // 默认按「不是新能力」处理，只有确认是加在自己身上的新能力时才置为 false  && amount > 0
            _hadPowerBefore = true;

            Creature? self = Owner?.Creature;
            if (self != null && target == self && power != null)
            {
                _hadPowerBefore = self.Powers.Contains(power);
            }

            return Task.CompletedTask;
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == null || amount <= 0) return;

            // 「新能力」：变化前没有这个能力；或者层数正好等于本次增加量（说明是刚被创建出来的）。&& power.Amount != amount
            // 后者是兜底，防止某些施加路径不经过 BeforePowerAmountChanged。
            if (_hadPowerBefore ) return;

            // 青溟剑势本身不算「新能力」，否则会自己触发自己
            if (power is QingmingJianshiPower || power is SupportPointPower) return;

            Creature? self = Owner?.Creature;
            if (self == null) return;

            // 明心镜状态下不触发
            if (self.HasPower<MingxinJingPower>()) return;

            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, self, 1, self, null);
        }

        // ==================== 使用卡牌计数 -> 加入终结技 ====================

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay?.Card?.Owner != Owner) return;

            // 手卡已有终结技时计数不累加（打出的如果是终结技本身，此时它已不在手卡里）
            if (!HasEndSkillInHand && Counter < Max) Counter++;

            await GiveEndSkillIfReady(context);
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            //if (Owner.Creature.CombatState?.RoundNumber != 1) return;

            // 战斗开始时也检查一次：计数是跨战斗保留的，可能开场就已经够了
            await GiveEndSkillIfReady(choiceContext);
        }

        /// <summary>计数达到阈值就把一张《逐云惊霆+》加入手卡</summary>
        private async Task GiveEndSkillIfReady(PlayerChoiceContext choiceContext)
        {
            if (Counter < Threshold) return;
            if (Owner.Creature.HasPower<MingxinJingPower>()) return;
            if (HasEndSkillInHand) return;   // 手里已经有一张终结技了，不重复发放
            if (Owner.Creature.CombatState is not { } combatState) return;

            Counter -= Threshold;

            Flash();

            CardModel reward = combatState.CreateCard<ZhuYunJingTing>(Owner);
            reward.UpgradeInternal();   // 遗物给的是升级后的《逐云惊霆+》
            await CardPileCmd.AddGeneratedCardToCombat(reward, PileType.Hand, Owner);
        }

        public void SetThreshold(int threshold) => Threshold = threshold;
        public void ResetThreshold() => Threshold = DefaultThreshold;

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            ResetThreshold();
        }
    }

    /// <summary>
    /// 青溟剑匣的升级版（Orobas精炼后）
    /// TODO: 替换为YeshunGuang专属效果
    /// </summary>
    [RegisterRelic(typeof(YeshunGuangRelicPool))]
    internal class QingmingJianRelic : ModRelicTemplate, IDecibleCounter
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/relics/notailFull33.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<MingCanXue>(),
            HoverTipFactory.FromPower<FrostFallPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill),
            HoverTipFactory.FromPower<StrengthPower>()
        ];

        public int Threshold { get; set; } = 30;
        public int Max { get; set; } = 30;

        private int _counter;

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

        public void AddCounter(int amount, bool forceAdd = false)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is MingCanXue);
            if (hasEnd) counter += Threshold;

            if (counter < Max || forceAdd)
                this.Counter += amount;
        }

        public void SetMax(int amount) => Max = amount;
        public void ResetMax() => Max = 30;

        public int GetCounter() => Counter;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            int counter = 0;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is MingCanXue);
            if (hasEnd) counter += Threshold;

            if (cardPlay.Card.Owner == base.Owner && counter < Max)
            {
                Counter++;

                if (Counter >= Threshold && !hasEnd)
                {
                    Counter -= Threshold;
                    if (Counter > Max - Threshold)
                        Counter = Max - Threshold;

                    Flash();

                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner);
                }
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<FrostFallPower>(choiceContext, base.Owner.Creature, 4, null, null);
            }
        }

        public void SetThreshold(int threshold) => Threshold = threshold;
        public void ResetThreshold() => Threshold = 30;

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            ResetThreshold();
        }
    }
}
