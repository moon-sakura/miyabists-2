using STS2RitsuLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    internal class AllForHupowangEvent : ModEventTemplate
    {
        // 背景图位置
        public override string? CustomInitialPortraitPath => "res://images/events/juFufu.png";

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            //new DamageVar(5m, ValueProp.Unblockable&ValueProp.Unpowered),
            //new StringVar("Relic", ModelDb.Relic<SanluoXingdianRelic>().Title.GetFormattedText()),
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
            new EventOption(this,AllForHupowangChoice,"ALL_FOR_HUPOWANG_CHOICE"),
            new EventOption(this,Others,"OTHERS"),
        ];

        private async Task AllForHupowangChoice()
        {
            CardModel card = base.Owner.RunState.CreateCard<AllForHupowang>(base.Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
            SetEventFinished(PageDescription("JUFUFU"));
        }

        private async Task Others()
        {
            await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], GetBreakCards), 3, Owner)]);
            SetEventFinished(PageDescription("OTHER_BREAKER"));
        }

        private bool GetBreakCards(CardModel card) 
        {
            if(card is ZuihuaYueyunzhuan) { return true; }
            if(card is SongKe) { return true; }
            if(card is CuteFeitianzhuang) { return true; }
            if(card is CiquanLianji) { return true; }
            if(card is MingfuWange) { return true; }
            return false;
        }

    }
}
