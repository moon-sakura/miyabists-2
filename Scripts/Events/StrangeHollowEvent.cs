using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    [RegisterSharedEvent]
    internal class StrangeHollowEvent : ModEventTemplate
    {
        // 背景图位置
        public override EventAssetProfile AssetProfile => new(
            InitialPortraitPath: "res://images/events/strangehollow.png"
        );

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars => [];

        // 什么时候会遇到。
        //public override bool IsAllowed(IRunState runState)
        //{
        //    return runState.Players.All(p => MiyabiFuncBase.IsMiyabiModChar(p));
        //}

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
            new EventOption(this, Explore, InitialOptionKey("EXPLORE")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE")),
        ];

        // ====== 选择"进去探索" → 进入奖励页面 ======
        private Task Explore()
        {
            SetRewardPage();
            return Task.CompletedTask;
        }

        // ====== 选择"远离" → 结束 ======
        private async Task Leave()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.LEAVE_PAGE.description"));
        }

        // ====== 奖励页面：三选一 ======
        private void SetRewardPage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.REWARD_PAGE.description"), [
                new EventOption(this, GetRareRelic, ModOptionKey("REWARD_PAGE", "GET_RELIC")),
                new EventOption(this, GetRareCard, ModOptionKey("REWARD_PAGE", "GET_CARD")),
                new EventOption(this, GetGold, ModOptionKey("REWARD_PAGE", "GET_GOLD")),
            ]);
        }

        // 获取随机一个稀有遗物
        private async Task GetRareRelic()
        {
            var relic = new RelicReward(RelicRarity.Rare, Owner);
            if (relic != null)
            {
                await RewardsCmd.OfferCustom(Owner!, [relic]);
            }
            // 奖励后进入结束页，加入诅咒卡
            await EnterEndPage();
        }

        // 从3张随机稀有卡牌中获取一张
        private async Task GetRareCard()
        {
            int result = Owner.PlayerRng.Rewards.NextInt(0, 3);
            switch (result)
            {
                case 0:
                    if(Owner.Character is Miyabi)
                    {
                        await MiyabiFuncBase.AddCardToDesk<FangfeiZhiyao>(base.Owner);
                    }
                    if(Owner.Character is Yixuan)
                    {
                        await MiyabiFuncBase.AddCardToDesk<ZhanwangKaitian>(base.Owner);
                    }
                    break;
                case 1:
                    if (Owner.Character is Miyabi)
                    {
                        await MiyabiFuncBase.AddCardToDesk<XuanheChenian>(base.Owner);
                    }
                    if (Owner.Character is Yixuan)
                    {
                        await MiyabiFuncBase.AddCardToDesk<XuanheChenian>(base.Owner);
                    }
                    break;
                case 2:
                    if (Owner.Character is Miyabi)
                    {
                        await MiyabiFuncBase.AddCardToDesk<AllForHupowang>(base.Owner);
                    }
                    if (Owner.Character is Yixuan)
                    {
                        await MiyabiFuncBase.AddCardToDesk<AllForHupowang>(base.Owner);
                    }
                    break; 
                default:
                    break;
            }
            await EnterEndPage();
        }

        // 获得240金币
        private async Task GetGold()
        {
            await PlayerCmd.GainGold(240m, Owner!);
            await EnterEndPage();
        }

        // 奖励后进入结束页：加入诅咒卡《空洞侵蚀》
        private async Task EnterEndPage()
        {
            await MiyabiFuncBase.AddCardToDesk<HollowErosion>(base.Owner);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.END_PAGE.description"));
        }

        // 过滤稀有卡牌
        private bool FilterANCards(CardModel card)
        {
            return card.Rarity == CardRarity.Ancient;
        }
    }
}
