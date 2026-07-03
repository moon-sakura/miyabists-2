using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    [RegisterSharedEvent]
    internal class OtherworldlyVoiceEvent : ModEventTemplate
    {
        private const int MaxCurseCount = 6;

        private int _curseCount = 0;

        // 背景图位置
        public override EventAssetProfile AssetProfile => new(
            InitialPortraitPath: "res://images/events/GetSlxd.png"
        );

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars => [];

        // 什么时候会遇到。
        public override bool IsAllowed(IRunState runState)
        {
            return runState.Players.All(p => MiyabiFuncBase.IsMiyabiModChar(p));
        }

        // 事件开始前的逻辑。
        protected override Task BeforeEventStarted(bool isPreFinished)
        {
            _curseCount = 0;
            return Task.CompletedTask;
        }

        // 事件结束后的逻辑。
        protected override void OnEventFinished()
        {

        }

        // 生成事件初始选项。
        protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            new EventOption(this, WantPower, InitialOptionKey("WANT_POWER")),
            new EventOption(this, Refuse, InitialOptionKey("REFUSE")),
        ];

        // 我想要力量 —— 给予第一张诅咒，进入"是否还要"页面
        private async Task WantPower()
        {
            _curseCount++;
            //CardModel card = base.Owner.RunState.CreateCard<PriceOfPower>(base.Owner);
            //await CardPileCmd.Add(card, PileType.Deck);
            //await CardPileCmd.AddCurseToDeck<PriceOfPower>(Owner);
            await MiyabiFuncBase.AddCardToDesk<PriceOfPower>(base.Owner);
            SetContinueAskPage();
        }

        // 拒绝（初始页） —— 进入拒绝页面
        private async Task Refuse()
        {
            SetRefusePage();
        }

        // "是否还要？"页面
        private void SetContinueAskPage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.CONTINUE_ASK.description"), [
                new EventOption(this, MorePower, ModOptionKey("CONTINUE_ASK", "MORE")),
                new EventOption(this, RefuseContinue, ModOptionKey("CONTINUE_ASK", "REFUSE")),
            ]);
        }

        // 力量多多益善 —— 再给一张诅咒
        private async Task MorePower()
        {
            _curseCount++;
            await MiyabiFuncBase.AddCardToDesk<PriceOfPower>(base.Owner);

            if (_curseCount >= MaxCurseCount)
            {
                // 拿满六张，进入贪婪结局
                SetGreedyEndPage();
            }
            else
            {
                // 还没满，继续问"是否还要"
                SetContinueAskPage();
            }
        }

        // 循环中拒绝 —— 直接结束
        private async Task RefuseContinue()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.END.description"));
        }

        // 贪婪结局页面（拿了六张诅咒）
        private void SetGreedyEndPage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.GREEDY_END.description"), [
                new EventOption(this, LeaveFromGreedy, ModOptionKey("GREEDY_END", "LEAVE")),
            ]);
        }

        // 从贪婪结局离开
        private async Task LeaveFromGreedy()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.GREEDY_LEAVE.description"));
        }

        // 拒绝页面（初始选择拒绝）
        private void SetRefusePage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.REFUSE_PAGE.description"), [
                new EventOption(this, LeaveFromRefuse, ModOptionKey("REFUSE_PAGE", "LEAVE")),
            ]);
        }

        // 从拒绝页面离开
        private async Task LeaveFromRefuse()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.REFUSE_LEAVE.description"));
        }
    }
}
