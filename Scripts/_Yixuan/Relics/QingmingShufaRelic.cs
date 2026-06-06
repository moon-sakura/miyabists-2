using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Miyabists2.Scripts._Yixuan.Relics
{
    [RegisterRelic(typeof(YixuanRelicPool))]
    [RegisterTouchOfOrobasRefinement(typeof(QingmingNiaoRelic))]
    internal class QingmingShufaRelic : ModRelicTemplate, IDecibleCounter
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        // TODO: 替换为Yixuan专属遗物图标
        public override string PackedIconPath => "res://images/relics/swordNotail200.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        // TODO: 替换为Yixuan专属触发卡和关键字
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<MingCanXue>(),
            HoverTipFactory.FromPower<ShannengPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill)
        ];

        public int Threshold { get; set; } = 30; // 触发阈值
        public int Max { get; set; } = 30;

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

        // 每次打出卡牌后检查
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            int counter = 0;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is MingCanXue);
            if (hasEnd) counter += Threshold;

            if (cardPlay.Card.Owner == base.Owner && counter < Max)
            {
                Counter++;

                // 检查是否达到阈值
                if (Counter >= Threshold && !hasEnd)
                {
                    Counter -= Threshold;
                    if (Counter > Max - Threshold)
                        Counter = Max - Threshold;

                    Flash();

                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                }
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                //第一回合添加能力
            }
        }
    }

    /// <summary>
    /// 清明书法遗物的升级版（Orobas精炼后）
    /// TODO: 替换为Yixuan专属效果
    /// </summary>
    [RegisterRelic(typeof(YixuanRelicPool))]
    internal class QingmingNiaoRelic : ModRelicTemplate, IDecibleCounter
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
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                }
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<ShannengPower>(choiceContext, base.Owner.Creature, 151, null, null);
            }
        }
    }
}
