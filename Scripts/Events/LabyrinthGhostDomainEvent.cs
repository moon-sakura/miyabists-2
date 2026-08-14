using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Enemies;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    [RegisterSharedEvent]
    internal class LabyrinthGhostDomainEvent : ModEventTemplate
    {
        // ===== 战斗概率参数 =====
        private const int InitialCombatChance = 20; // 初始战斗概率
        private const int CombatChancePerStep = 5;  // 每次选择继续但未遇到猎犬时 +5%
        private const int BaseCombatChanceCap = 40; // 初始概率上限

        // 已战胜猎犬的次数（决定奖励档位与猎犬强度）
        private int _houndEncounters = 0;

        // 当前累计战斗概率 & 初始战斗概率
        private int _baseCombatChance = InitialCombatChance;
        private int _combatChance = InitialCombatChance;

        // 背景图位置（暂用占位，可替换成专属背景）
        public override EventAssetProfile AssetProfile => new(
            InitialPortraitPath: "res://images/events/dogEvent.png"
        );

        // 设置一些数值
        protected override IEnumerable<DynamicVar> CanonicalVars => [];

        // 什么时候会遇到。
        public override bool IsAllowed(IRunState runState)
        {
            return runState.Players.All(p => MiyabiFuncBase.IsMiyabiModChar(p)) && runState.Players.Count == 1;
        }

        // 事件开始前重置状态。
        protected override Task BeforeEventStarted(bool isPreFinished)
        {
            _houndEncounters = 0;
            _baseCombatChance = InitialCombatChance;
            _combatChance = InitialCombatChance;
            return Task.CompletedTask;
        }

        public override bool IsShared => true;

        // 事件结束后的逻辑。
        protected override void OnEventFinished()
        {

        }

        // 生成事件初始选项。
        protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
        [
            new EventOption(this, Accept, InitialOptionKey("ACCEPT")),
            new EventOption(this, Refuse, InitialOptionKey("REFUSE")),
        ];

        // ====== 同意 → 进入同意页面 ======
        private Task Accept()
        {
            SetAgreePage();
            return Task.CompletedTask;
        }

        // ====== 拒绝 → 结束 ======
        private async Task Refuse()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.REFUSE_END.description"));
        }

        // ====== 同意页面 ======
        private void SetAgreePage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.AGREE_PAGE.description"), [
                new EventOption(this, ContinueAdvance, ModOptionKey("AGREE_PAGE", "ADVANCE")),
            ]);
        }

        // ====== 前进 → 进入页面A ======
        private Task GoPageA()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.PAGE_A.description"), [
                new EventOption(this, ContinueAdvance, ModOptionKey("PAGE_A", "CONTINUE")),
                new EventOption(this, Retreat, ModOptionKey("PAGE_A", "RETREAT")),
            ]);
            return Task.CompletedTask;
        }

        // ====== 继续前进：获取随机奖励 → 判定是否触发战斗 ======
        private async Task ContinueAdvance()
        {
            await GiveRandomReward();

            if (RollCombatChance())
            {
                SetCombatPage();
            }
            else
            {
                GoPageA();
            }
        }

        // ====== 撤离 → 结束 ======
        private async Task Retreat()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.RETREAT_END.description"));
        }

        // ======================== 奖励档位系统 ========================
        private enum RewardKind { CommonCard, UncommonCard, RareCard, AncientCard, Potion, Relic, Gold, Curse }

        // 奖励档位：遇到猎犬0次=1档，1次=2档，2次=3档，3次=4档，4次及以上=5档
        private int GetRewardTier() => _houndEncounters switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            3 => 4,
            _ => 5,
        };

        // 每档权重表（权重和=100），可自行调整
        private static (RewardKind Kind, int Weight)[] GetRewardTable(int tier) => tier switch
        {
            // 1档：普通卡牌、药水、遗物、少量金币为主，小概率Uncommon/Rare
            1 =>
            [
                (RewardKind.CommonCard, 25), (RewardKind.UncommonCard, 6), (RewardKind.RareCard, 3),
                (RewardKind.Potion, 22), (RewardKind.Relic, 19), (RewardKind.Gold, 15), (RewardKind.Curse, 10),
            ],
            // 2档：Uncommon为主，普通稍少，Rare更高，极小概率Ancient，金币上升
            2 =>
            [
                (RewardKind.CommonCard, 15), (RewardKind.UncommonCard, 25), (RewardKind.RareCard, 9), (RewardKind.AncientCard, 2),
                (RewardKind.Potion, 18), (RewardKind.Relic, 13), (RewardKind.Gold, 13), (RewardKind.Curse, 5),
            ],
            // 3档：Uncommon和Rare为主，普通大幅下降，小概率Ancient，金币再次上升
            3 =>
            [
                (RewardKind.CommonCard, 8), (RewardKind.UncommonCard, 25), (RewardKind.RareCard, 20), (RewardKind.AncientCard, 4),
                (RewardKind.Potion, 15), (RewardKind.Relic, 12), (RewardKind.Gold, 13), (RewardKind.Curse, 3),
            ],
            // 4档：Rare为主，Uncommon少见，普通极少见，Ancient概率稍微上升，金币再次上升
            4 =>
            [
                (RewardKind.CommonCard, 4), (RewardKind.UncommonCard, 11), (RewardKind.RareCard, 30), (RewardKind.AncientCard, 7),
                (RewardKind.Potion, 12), (RewardKind.Relic, 14), (RewardKind.Gold, 20), (RewardKind.Curse, 2),
            ],
            // 5档：Rare为主，Uncommon少见，Ancient概率上升，金币大量
            _ =>
            [
                (RewardKind.CommonCard, 3), (RewardKind.UncommonCard, 8), (RewardKind.RareCard, 32), (RewardKind.AncientCard, 12),
                (RewardKind.Potion, 10), (RewardKind.Relic, 12), (RewardKind.Gold, 22), (RewardKind.Curse, 1),
            ],
        };

        // ====== 获取随机奖励（按档位） ======
        private async Task GiveRandomReward()
        {
            int tier = GetRewardTier();
            var table = GetRewardTable(tier);

            int roll = Owner.PlayerRng.Rewards.NextInt(0, 100);
            RewardKind kind = RewardKind.Gold;
            int cumulative = 0;
            foreach (var (k, w) in table)
            {
                cumulative += w;
                if (roll < cumulative) { kind = k; break; }
            }

            switch (kind)
            {
                case RewardKind.CommonCard:
                    if (!await TryOfferCard(CardRarity.Common)) await GainFallbackGold(tier);
                    break;
                case RewardKind.UncommonCard:
                    if (!await TryOfferCard(CardRarity.Uncommon)) await GainFallbackGold(tier);
                    break;
                case RewardKind.RareCard:
                    if (!await TryOfferCard(CardRarity.Rare)) await GainFallbackGold(tier);
                    break;
                case RewardKind.AncientCard:
                    if (!await TryOfferCard(CardRarity.Ancient)) await GainFallbackGold(tier);
                    break;
                case RewardKind.Potion:
                    if (!await TryOfferPotion(RollPotionRarity(tier))) await GainFallbackGold(tier);
                    break;
                case RewardKind.Relic:
                    if (!await TryOfferRelic(RollRelicRarity(tier))) await GainFallbackGold(tier);
                    break;
                case RewardKind.Gold:
                    await PlayerCmd.GainGold(RollGold(tier), Owner!);
                    break;
                case RewardKind.Curse:
                    await MiyabiFuncBase.AddCardToDesk<HollowErosion>(base.Owner);
                    break;
            }
        }

        // ====== 奖励实现 ======

        // 从该稀有度的卡牌中选择（卡池为空则返回false，改给金币保底）
        // 注意：不能走 CardFactory.CreateForReward 生成奖励！它先按稀有度概率 roll 出一个稀有度，
        // 再在卡池里找该稀有度的卡。本角色卡池里只有 Ancient 卡（煊赫车辇、斩妄开天），
        // 而 RegularEncounter 概率永远只 roll 出 Common/Uncommon/Rare，GetNextAllowedRarity
        // 的回绕链（Common→Uncommon→Rare→Common，Ancient→None）永远到不了 Ancient，
        // 所以第一张卡就抛 "couldn't generate a valid rarity"。
        // 解决办法：自己选好卡、用卡片版构造器直接塞给 CardReward（_cardsWereManuallySet 路径，
        // Populate 不再调 CardFactory，也不会因卡池不足/稀有度 roll 失败而崩）。
        private async Task<bool> TryOfferCard(CardRarity rarity)
        {
            List<CardModel> unlocked = Owner!.Character.CardPool
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Rarity == rarity && c.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly && c is not XuanmoAnyong)
                .Distinct()
                .ToList();
            if (unlocked.Count == 0)
            {
                return false;
            }
            int offerCount = Math.Min(3, unlocked.Count);
            List<CardModel> cardsToOffer = unlocked
                .TakeRandom(offerCount, Owner.PlayerRng.Rewards)
                .Select(c => Owner.RunState.CreateCard(c, Owner))
                .ToList();
            await RewardsCmd.OfferCustom(Owner!, [
                new CardReward(cardsToOffer, CardCreationSource.Other, Owner, CardCreationOptions.ForNonCombatWithDefaultOdds([]))
            ]);
            return true;
        }

        // 给予随机药水（该稀有度无药水则返回false，改给金币保底）
        private async Task<bool> TryOfferPotion(PotionRarity rarity)
        {
            IEnumerable<PotionModel> items = Owner!.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState)
                .Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState))
                .Where(p => p.Rarity == rarity);
            PotionModel potion = Owner.PlayerRng.Rewards.NextItem(items);
            if (potion == null) return false;
            await RewardsCmd.OfferCustom(Owner!, [new PotionReward(potion.ToMutable(), Owner)]);
            return true;
        }

        // 给予随机遗物（遗物池耗尽时会返回花环Circlet，视为耗尽改给金币保底）
        private async Task<bool> TryOfferRelic(RelicRarity rarity)
        {
            var relic = new RelicReward(rarity, Owner);
            if (relic == null || relic.Relic is Circlet) return false;
            await RewardsCmd.OfferCustom(Owner!, [relic]);
            return true;
        }

        // 根据档位决定药水稀有度
        private PotionRarity RollPotionRarity(int tier) => tier switch
        {
            1 => PotionRarity.Common,
            2 => Owner.PlayerRng.Rewards.NextInt(0, 2) == 0 ? PotionRarity.Common : PotionRarity.Uncommon,
            3 => PotionRarity.Uncommon,
            4 => Owner.PlayerRng.Rewards.NextInt(0, 2) == 0 ? PotionRarity.Uncommon : PotionRarity.Rare,
            _ => PotionRarity.Rare,
        };

        // 根据档位决定遗物稀有度
        private RelicRarity RollRelicRarity(int tier) => tier switch
        {
            1 => RelicRarity.Common,
            2 => RelicRarity.Uncommon,
            3 => Owner.PlayerRng.Rewards.NextInt(0, 2) == 0 ? RelicRarity.Uncommon : RelicRarity.Rare,
            4 => Owner.PlayerRng.Rewards.NextInt(0, 3) == 0 ? RelicRarity.Uncommon : RelicRarity.Rare,
            _ => Owner.PlayerRng.Rewards.NextInt(0, 4) == 0 ? RelicRarity.Uncommon : RelicRarity.Rare,
        };

        // 金币数量随档位上升（1档少量 → 5档大量）
        private int RollGold(int tier) => tier switch
        {
            1 => Owner.PlayerRng.Rewards.NextInt(20, 41),    // 少量
            2 => Owner.PlayerRng.Rewards.NextInt(45, 71),
            3 => Owner.PlayerRng.Rewards.NextInt(70, 106),
            4 => Owner.PlayerRng.Rewards.NextInt(105, 161),
            _ => Owner.PlayerRng.Rewards.NextInt(160, 251),   // 大量
        };

        // 保底金币
        private Task GainFallbackGold(int tier) => PlayerCmd.GainGold(25m * tier, Owner!);

        // ======================== 战斗概率 ========================

        // 初始20%，每次选择继续但未遇到猎犬 +5%；遇到猎犬后清空累计并令初始概率+5%（上限40%）
        private bool RollCombatChance()
        {
            bool trigger = Owner.PlayerRng.Rewards.NextInt(1, 101) <= _combatChance;
            if (trigger)
            {
                _baseCombatChance = Math.Min(_baseCombatChance + CombatChancePerStep, BaseCombatChanceCap);
                _combatChance = _baseCombatChance;
            }
            else
            {
                _combatChance += CombatChancePerStep;
            }
            return trigger;
        }

        // ====== 战斗页面 ======
        private void SetCombatPage()
        {
            SetEventState(L10NLookup($"{Id.Entry}.pages.COMBAT_PAGE.description"), [
                new EventOption(this, FightHound, ModOptionKey("COMBAT_PAGE", "FIGHT")),
            ]);
        }

        // ====== 进入战斗 ======
        private Task FightHound()
        {
            // 遭遇必须传 canonical 模型：框架进入战斗时会自己 ToMutable() 生成战斗副本，
            // 传可变副本会触发 "Mutable model used in incorrect place" 报错。
            // 战斗次数通过静态字段传给猎犬（canonical 实例无法携带战斗临时数据）。
            //
            // 框架的 EventCombatSynchronizer 只支持一个事件实例进入一次战斗：玩家的"就绪"状态
            // （ReadyToEnterCombat）只在离开事件房间时（EventRoom.Exit → ResetState）才清空，
            // 同一次事件里再次调用 EnterCombatWithoutExitingEvent 会报 "already set to ready"。
            // 本事件需要多次战斗，因此每次进入战斗前手动重置同步器并重新初始化。
            _combatSynchronizer?.ResetState();
            _combatSynchronizer?.InitializeForEvent(this);

            UridimuHoundEncounter.NextEncounterIndex = _houndEncounters + 1;

            // 战斗回放记录器是整局共享的：初始状态只在进入地图节点时记录（EnterMapPointInternal），
            // 事件战斗走的是 EnterRoomWithoutExitingCurrentRoom，不会记录。且每次战斗胜利后
            // WriteReplay(stopRecording:true) 会清空记录，所以第 2 次及之后的战斗必须先重新记录初始状态，
            // 否则战斗中第一次生成校验和就会抛 "RecordInitialState must be called first" 并卡死战斗。
            RunManager.Instance.CombatReplayWriter.RecordInitialState(RunManager.Instance.ToSave(null));

            EnterCombatWithoutExitingEvent<UridimuHoundEncounter>([], shouldResumeAfterCombat: true);
            return Task.CompletedTask;
        }

        // ====== 战斗结束后的处理 ======
        public override async Task Resume(AbstractRoom exitedRoom)
        {
            // 战斗胜利（未死亡）才继续，否则事件直接结束
            if (Owner.Creature.CurrentHp <= 0) return;

            // 猎犬主动逃跑了：不计入战胜次数、不给猎犬遗骸遗物，正常回到事件页面A
            CombatRoom combatRoom = (CombatRoom)exitedRoom;
            UridimuHoundEncounter uridimuHoundEncounter = (UridimuHoundEncounter)combatRoom.Encounter;
            if (!uridimuHoundEncounter.RanOutOfTime)
            {
                // 第四次战胜猎犬后获得专属遗物（猎犬遗骸，只会获得一次）
                if (_houndEncounters >= 4 && MiyabiFuncBase.GetRelic<HoundRemainsRelic>(Owner) == null)
                {
                    //await RelicCmd.Obtain<HoundRemainsRelic>(Owner);
                    var relic = new RelicReward(ModelDb.Relic<HoundRemainsRelic>(), Owner);
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                }
            }
            _houndEncounters++;

            SetEventState(L10NLookup($"{Id.Entry}.pages.POST_COMBAT_PAGE.description"), [
                new EventOption(this, ContinueAdvance, ModOptionKey("POST_COMBAT_PAGE", "CONTINUE")),
                new EventOption(this, Retreat, ModOptionKey("POST_COMBAT_PAGE", "RETREAT")),
            ]);
        }
    }
}
