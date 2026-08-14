using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    // 猎犬遗骸：第四次战胜旧日猎犬·乌利迪姆后获得（只会获得一次）。
    // 效果：战斗结束之后，额外获得一份随机奖励——随机稀有度的药水 / 卡牌 / 遗物 / 金币。
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class HoundRemainsRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override string PackedIconPath => "res://images/relics/dogRelic.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        // 战斗胜利后，额外获得一份随机奖励。
        // 注意：不能放在 BeforeRoomEntered / AfterRoomEntered——进入下一场战斗的房间时也会触发，
        // RewardsCmd.OfferCustom 会在战斗开始前弹出奖励界面，把战斗卡死（之前就是这么卡的）。
        // AfterCombatVictory 在战斗胜利后（奖励/转场前）触发，此时弹奖励界面是安全的。
        public override async Task AfterCombatVictory(CombatRoom room)
        {
            await base.AfterCombatVictory(room);
            await GrantRandomBonusReward();
        }

        private async Task GrantRandomBonusReward()
        {
            int kind = Owner.PlayerRng.Rewards.NextInt(0, 4);
            switch (kind)
            {
                case 0: // 随机稀有度的药水
                    if (!await TryOfferPotion(RandomPotionRarity()))
                        await GainFallbackGold();
                    break;
                case 1: // 随机稀有度的卡牌（3选1）
                    if (!await TryOfferCard(RandomCardRarity()))
                        await GainFallbackGold();
                    break;
                case 2: // 随机稀有度的遗物
                    if (!await TryOfferRelic(RandomRelicRarity()))
                        await GainFallbackGold();
                    break;
                case 3: // 金币
                    await PlayerCmd.GainGold(Owner.PlayerRng.Rewards.NextInt(30, 71), Owner);
                    break;
            }
        }

        private static readonly PotionRarity[] PotionRarities = [PotionRarity.Common, PotionRarity.Uncommon, PotionRarity.Rare];
        private PotionRarity RandomPotionRarity() => Owner.PlayerRng.Rewards.NextItem(PotionRarities);

        private static readonly RelicRarity[] RelicRarities = [RelicRarity.Common, RelicRarity.Uncommon, RelicRarity.Rare];
        private RelicRarity RandomRelicRarity() => Owner.PlayerRng.Rewards.NextItem(RelicRarities);

        private static readonly CardRarity[] CardRarities = [CardRarity.Common, CardRarity.Uncommon, CardRarity.Rare, CardRarity.Ancient];
        private CardRarity RandomCardRarity() => Owner.PlayerRng.Rewards.NextItem(CardRarities);

        private async Task<bool> TryOfferPotion(PotionRarity rarity)
        {
            IEnumerable<PotionModel> items = Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState)
                .Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState))
                .Where(p => p.Rarity == rarity);
            PotionModel potion = Owner.PlayerRng.Rewards.NextItem(items);
            if (potion == null) return false;
            await RewardsCmd.OfferCustom(Owner, [new PotionReward(potion.ToMutable(), Owner)]);
            return true;
        }

        private async Task<bool> TryOfferCard(CardRarity rarity)
        {
            // 注意：不能走 CardFactory.CreateForReward（即 CardReward 带 Odds 的构造器）！
            // 它先按稀有度概率 roll 出一个稀有度，再在卡池里找该稀有度的卡。本角色卡池只有
            // Ancient 卡（煊赫车辇、斩妄开天），而 RegularEncounter 概率永远只 roll 出
            // Common/Uncommon/Rare，GetNextAllowedRarity 的回绕链永远到不了 Ancient，
            // 第一张卡就抛 "couldn't generate a valid rarity"。跟迷宫诡域事件一样，改为自己选好卡、
            // 用卡片版构造器直接塞给 CardReward（_cardsWereManuallySet 路径，不再调 CardFactory）。
            List<CardModel> unlocked = Owner.Character.CardPool
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Rarity == rarity && c.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly && c is not XuanmoAnyong && c is not NoTailFull)
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
            await RewardsCmd.OfferCustom(Owner, [
                new CardReward(cardsToOffer, CardCreationSource.Other, Owner, CardCreationOptions.ForNonCombatWithDefaultOdds([]))
            ]);
            return true;
        }

        private async Task<bool> TryOfferRelic(RelicRarity rarity)
        {
            // 保底：遗物池耗尽（RelicReward 会返回花环 Circlet）时，改为金币
            var relic = new RelicReward(rarity, Owner);
            if (relic == null || relic.Relic is Circlet) return false;
            await RewardsCmd.OfferCustom(Owner, [relic]);
            return true;
        }

        private Task GainFallbackGold() => PlayerCmd.GainGold(25m, Owner);
    }
}
