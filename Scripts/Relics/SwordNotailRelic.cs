using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System.Diagnostics.Metrics;

namespace Miyabists2.Scripts.Relics
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class SwordNotailRelic : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/relics/commonRelics.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<MingCanXue>()];

        public int Threshold {get;set;} = 30; // 触发阈值

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
                AssertMutable(); // 确保在合法的修改状态
                _counter = value;
                InvokeDisplayAmountChanged(); // 通知 UI 更新数字
            }
        }

        //public static void AddCounter(int counter)
        //{
        //    SwordNotailRelic.Counter += counter;
        //}

        public void AddCounter(int amount)
        {
            // 这里在类内部，可以访问 private set
            this.Counter += amount;
            //this.Flash(); // 让遗物闪烁一下，视觉效果更好
        }

        // 每次打出卡牌后检查
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            // 1. 检查是否是特定的卡（或者任意卡，根据你的需求）
            // 如果是特定卡，可以检查 cardPlay.Card.Id == "你的卡ID"
            if (cardPlay.Card.Owner == base.Owner)
            {
                Counter++;

                // 2. 检查是否达到 30 次
                if (Counter >= Threshold)
                {
                    Counter = 0; // 重置计数器

                    // 3. 触发效果：闪烁并加入一张卡
                    Flash();

                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
                }
            }
        }



        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature, 4, null, null);
            }
            // 此时，syncRnd.Next 在所有客户端产生的结果将完全一致
            int result = base.Owner.RunState.Rng.Shuffle.NextInt(1, 4); ;
        }

        //public override Task AfterCombatEnd(CombatRoom _)
        //{
        //    base.Status = RelicStatus.Normal;
        //    return Task.CompletedTask;
        //}
    }
}
