using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    internal class GetSlxdEvent : CustomEventModel
    {
        // 背景图位置
        public override string? CustomInitialPortraitPath => "res://images/events/commonEvents.png";

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Unblockable&ValueProp.Unpowered),
            new StringVar("Relic", ModelDb.Relic<SanluoXingdianRelic>().Title.GetFormattedText()),
        ];

        // 什么时候会遇到。
        public override bool IsAllowed(IRunState runState)
        {
            return runState.Players.All(p => p.Character is Miyabi);
        }

        // 事件开始前的逻辑。
        protected override Task BeforeEventStarted(bool isPreFinished)
        {
            return Task.CompletedTask;
        }

        // 事件结束后的逻辑。
        protected override void OnEventFinished()
        {
            
        }

        // 生成事件初始选项。
        protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            Option(DeepInHeart),
            Option(Leave),
        ];

        // 深入
        private async Task DeepInHeart()
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.Damage, null, null);
            CountinueDeepPage();
        }

        // 离开，结束
        private async Task Leave()
        {
            //await PlayerCmd.LoseGold(DynamicVars.Gold.BaseValue, Owner!, GoldLossType.Stolen);
            SetEventFinished(PageDescription("LEAVE"));
        }

        // 进入事件第二阶段
        private void CountinueDeepPage()
        {
            SetEventState(PageDescription("DEEP_PAGE"), [
                Option(ContinueDeep, "DEEP_PAGE"), // 第二个参数代表该选项所在页面
                Option(CannotBare, "DEEP_PAGE"),
                ]);
        }

        // 继续深入
        private async Task ContinueDeep()
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.Damage, null, null);
            ReachCorePage();
            
        }

        // 无法承受，从随机五张卡中选一张离开，结束
        private async Task CannotBare()
        {
            await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool]), 5, Owner)]);
            SetEventFinished(PageDescription("CANNOT_BARE_LEAVE"));
        }

        private void ReachCorePage() 
        {
            SetEventState(PageDescription("CORE_PAGE"), [
                Option(GetSlxd, "CORE_PAGE"),
                Option(GiveUp,"CORE_PAGE")
                ]);
        }

        //进入战斗（暂定），获得散落星殿
        private async Task GetSlxd()
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.Damage, null, null);
            await RelicCmd.Obtain<SanluoXingdianRelic>(base.Owner);
            SetEventFinished(PageDescription("GET_SLXD"));
        }

        //放弃，从三张Rare卡中选一张，结束
        private async Task GiveUp()
        {
            await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool],FilterRareCards), 3, Owner)]);
            SetEventFinished(PageDescription("GIVE_UP_LEAVE"));
        }

        private bool FilterRareCards(CardModel card)
        {
            return card.Rarity == MegaCrit.Sts2.Core.Entities.Cards.CardRarity.Rare;
        }
    }
}
