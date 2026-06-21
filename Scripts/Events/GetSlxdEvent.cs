using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
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
    [RegisterSharedEvent]
    internal class GetSlxdEvent : ModEventTemplate
    {
        // 背景图位置
        public override EventAssetProfile AssetProfile => new(
            InitialPortraitPath: "res://images/events/GetSlxd.png"
        );

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Unblockable|ValueProp.Unpowered),
            new StringVar("Relic", ModelDb.Relic<SanluoXingdianRelic>().Title.GetFormattedText()),
        ];

        public override bool IsShared => true;

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
            new EventOption(this, DeepInHeart, InitialOptionKey("DEEP_IN_HEART")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE")),
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
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.LEAVE.description"));
        }

        // 进入事件第二阶段
        private void CountinueDeepPage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.DEEP_PAGE.description"), [
                new EventOption(this, ContinueDeep, ModOptionKey("DEEP_PAGE", "CONTINUE_DEEP")),
                new EventOption(this, CannotBare, ModOptionKey("DEEP_PAGE", "CANNOT_BARE")),
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
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CANNOT_BARE_LEAVE.description"));
        }

        private void ReachCorePage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.CORE_PAGE.description"), [
                new EventOption(this, GetSlxd, ModOptionKey("CORE_PAGE", "GET_SLXD")),
                new EventOption(this, GiveUp, ModOptionKey("CORE_PAGE", "GIVE_UP"))
                ]);
        }

        //进入战斗，获得散落星殿
        private Task GetSlxd()
        {
            EnterCombatWithoutExitingEvent<MiyabiTestEncounter>([], shouldResumeAfterCombat: true);
            return Task.CompletedTask;
        }

        public override async Task Resume(AbstractRoom exitedRoom)
        {
            await RelicCmd.Obtain<SanluoXingdianRelic>(base.Owner);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.GET_SLXD.description"));
        }

        //放弃，从三张Rare卡中选一张，结束
        private async Task GiveUp()
        {
            await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.ForNonCombatWithDefaultOdds([Owner!.Character.CardPool],FilterRareCards), 3, Owner)]);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.GIVE_UP_LEAVE.description"));
        }

        private bool FilterRareCards(CardModel card)
        {
            return card.Rarity == MegaCrit.Sts2.Core.Entities.Cards.CardRarity.Rare;
        }
    }
}
