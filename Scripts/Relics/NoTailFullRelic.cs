using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class NoTailFullRelic : ModRelicTemplate, IDecibleCounter
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
            HoverTipFactory.FromCard<NoTailFull>(),
            HoverTipFactory.FromPower<StrengthPower>()
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

        //public static void AddCounter(int counter)
        //{
        //    SwordNotailRelic.Counter += counter;
        //}

        public void AddCounter(int amount, bool forceAdd = false)
        {
            // 这里在类内部，可以访问 private set
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is MingCanXue);
            if (hasEnd) counter += Threshold;

            if (counter < Max || forceAdd)
                this.Counter += amount;
            //this.Flash(); // 让遗物闪烁一下，视觉效果更好
        }

        public void SetMax(int amount) => Max = amount;
        public void ResetMax() => Max = 30;

        // 每次打出卡牌后检查
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is MingCanXue);
            // 1. 检查是否是特定的卡（或者任意卡，根据你的需求）
            // 如果是特定卡，可以检查 cardPlay.Card.Id == "你的卡ID"
            if (hasEnd) counter += Threshold;

            if (cardPlay.Card.Owner == base.Owner && counter < Max)
            {
                Counter++;

                // 2. 检查是否达到 30 次
                if (Counter >= Threshold && !hasEnd)
                {
                    Counter -= Threshold; // 重置计数器
                    if (Counter > Max - Threshold)
                        Counter = Max - Threshold;

                    // 3. 触发效果：闪烁并加入一张卡
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
                await PowerCmd.Apply<FrostFallPower>(choiceContext, base.Owner.Creature, 4, null, null);

                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<NoTailFull>(base.Owner.Creature.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
            }
        }

        
    }
}
