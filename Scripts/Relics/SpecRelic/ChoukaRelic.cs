using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts._Yixuan.Powers.CinimaPower;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Enchantment;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics.SpecRelic
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class ChoukaRelic : ModRelicTemplate, IRelicExtraIconAmountLabelSpecsProvider
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/relics/choukaRelic.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected int _counter = 0;

        // 显示在遗物图标上的数字
        public override bool ShowCounter => true;
        public override int DisplayAmount => Counter;

        public IReadOnlyList<ExtraIconAmountLabelSpec> GetRelicExtraIconAmountLabelSpecs()
        {
            return
            [
                ExtraIconAmountLabelSpec.RichText(
                ExtraIconAmountLabelCorner.TopRight,
                "[color=gold]"+CinimaCounter.ToString()+"[/color]"),
                ExtraIconAmountLabelSpec.RichText(
                ExtraIconAmountLabelCorner.BottomLeft,
                "[color=aqua]"+FreeCounter.ToString()+"[/color]"),
            ];
        }

        [SavedProperty]
        public int Counter
        {
            get => _counter;
            protected set
            {
                AssertMutable(); // 确保在合法的修改状态
                _counter = value;
                InvokeDisplayAmountChanged(); // 通知 UI 更新数字
            }
        }

        public void AddCounter(int amount)
        {
            this.Counter += amount;
            //this.Flash(); // 让遗物闪烁一下，视觉效果更好
        }

        protected int _freeCounter = 0;

        [SavedProperty]
        public int FreeCounter
        {
            get => _freeCounter;
            protected set
            {
                AssertMutable();
                _freeCounter = value;
                InvokeDisplayAmountChanged();
            }
        }

        protected int _cinimaCounter = 0;

        [SavedProperty]
        public int CinimaCounter
        {
            get => _cinimaCounter;
            protected set
            {
                AssertMutable();
                _cinimaCounter = value;
                InvokeDisplayAmountChanged();
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CINIMA",0),
            new DynamicVar("Uppercount", 25m),
            new DynamicVar("FreeCount", 0m)
        ];

        public override Task AfterRoomEntered(AbstractRoom room)
        {
            DynamicVars["FreeCount"].BaseValue = FreeCounter;
            DynamicVars["CINIMA"].BaseValue = CinimaCounter;
            return base.AfterRoomEntered(room);
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }

            DynamicVars["CINIMA"].BaseValue = CinimaCounter;

            if (base.Owner.Creature.CombatState.RoundNumber == 1 && Owner.Character is Miyabi)
            {
                Flash();
                if (CinimaCounter >= 1)
                {
                    await PowerCmd.Apply<XsjsPower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }

                if (CinimaCounter >= 2)
                {
                    await PowerCmd.Apply<TunafaPower>(choiceContext, base.Owner.Creature, 3m, null, null);
                }

                if (CinimaCounter >= 3)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 4m, null, null);
                }

                if (CinimaCounter >= 4)
                {
                    await PowerCmd.Apply<JunLiePower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }

                if (CinimaCounter >= 5)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 6m, null, null);
                }

                if (CinimaCounter >= 6)
                {
                    await PowerCmd.Apply<TianzizyPower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }
            }

        }

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
        {
            if (creature.IsMonster && creature.PetOwner == null && (int)MiyabiModConfig.CombatHardSelected >= 8)
            {
                FreeCounter++;
                DynamicVars["FreeCount"].BaseValue = FreeCounter;
            }
        }

        //public override Task BeforeRoomEntered(AbstractRoom room)
        //{
        //    FreeCounter++;
        //    DynamicVars["FreeCount"].BaseValue++;
        //    return base.BeforeRoomEntered(room);
        //}

        public override Task AfterCombatVictory(CombatRoom room)
        {
            FreeCounter++;
            DynamicVars["FreeCount"].BaseValue = FreeCounter;
            return base.AfterCombatVictory(room);
        }


        public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal originalPrice)
        {
            if (player != base.Owner)
            {
                return originalPrice;
            }
            if (!LocalContext.IsMe(base.Owner))
            {
                return originalPrice;
            }
            return originalPrice;// * (base.DynamicVars["Uppercount"].BaseValue / 100m) + originalPrice;
        }

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != base.Owner)
            {
                return false;
            }

            options.Add(new ChoukaRestSiteOption(player));
            return true;
        }

        protected bool _isBusy = false;
        protected int _lastCinima = 0;
        protected int _lastRare = 0;

        [SavedProperty]
        public int LastCinima
        {
            get => _lastCinima;
            protected set
            {
                AssertMutable();
                _lastCinima = value;
            }
        }

        [SavedProperty]
        public int LastRare
        {
            get => _lastRare;
            protected set
            {
                AssertMutable();
                _lastRare = value;
            }
        }

        public async Task OnUsed()
        {
            bool istest = false;


            //istest = true;

            DynamicVars["FreeCount"].BaseValue = FreeCounter;
            if (Owner.Gold < ChoukaRestSiteOption.Cost && DynamicVars["FreeCount"].BaseValue < 1)
            {
                return;
            }
            if (istest)
            {
                return;
            }

            if (!_isBusy)
            {
                try
                {
                    bool _hasDone = false;
                    _isBusy = true;

                    Flash();
                    AddCounter(1);
                    if (DynamicVars["FreeCount"].BaseValue >= 1)
                    {
                        DynamicVars["FreeCount"].BaseValue -= 1;
                        FreeCounter--;
                    }
                    else
                    {
                        Owner.Gold -= ChoukaRestSiteOption.Cost;
                    }

                    if (Counter - LastCinima >= 80)
                    {
                        if (CinimaCounter < 6)
                        {
                            CinimaCounter += 1;
                            DynamicVars["CINIMA"].BaseValue = CinimaCounter;
                        }
                        else
                        {
                            await AncientRewards();
                        }

                        LastRare = Counter;
                        LastCinima = Counter;
                        _hasDone = true;
                    }

                    int result = MiyabiFuncBase.RandomInt(0, 100, Owner);
                    if (CinimaCounter < 6 && !_hasDone)
                    {
                        if (result <= 1)
                        {
                            CinimaCounter++;
                            DynamicVars["CINIMA"].BaseValue = CinimaCounter;
                            LastCinima = Counter;
                            LastRare = Counter;
                            _hasDone = true;
                        }
                    }
                    if (result <= 9 && !_hasDone)
                    {
                        await AncientRewards();
                        LastRare = Counter;
                        _hasDone = true;
                    }
                    if ((result <= 19 && !_hasDone)|| Counter - LastRare >= 10)
                    {
                        await RareRewards();
                        LastRare = Counter;
                        _hasDone = true;
                    }
                    if (result <= 49 && !_hasDone)
                    {
                        await UncommonRewards();
                        _hasDone = true;
                    }
                    if (!_hasDone)
                    {
                        await CommonRewards();
                        _hasDone = true;
                    }

                    //_hasDone = false;
                }
                finally
                {
                    _isBusy = false;
                }
            }
            
        }

        protected async Task AncientRewards()
        {
            int result = MiyabiFuncBase.RandomInt(0, 9, Owner);
            bool rewardGiven = false;

            if (result == 0)
            {
                int enchantResult = MiyabiFuncBase.RandomInt(0, 3, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Instinct>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 1 && !rewardGiven)
                {
                    if (await TryEnchantCard<TezcatarasEmber>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 2 && !rewardGiven)
                {
                    if (await TryEnchantCard<BeeGroupEnchantment>())
                    {
                        rewardGiven = true;
                    }
                }
            }

            if (result == 1)
            {
                var relic = new RelicReward(RelicRarity.Ancient | RelicRarity.Shop, Owner);
                if (relic != null && relic.Relic is not Circlet)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    rewardGiven = true;
                }
            }

            if (result == 2)
            {
                List<CardModel> cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 3))).ToList();
                if (cardModel.Count > 0) 
                {
                    foreach (var card in cardModel)
                        CardCmd.Upgrade(card);
                    rewardGiven = true;
                }
            }

            if (result <= 3 && !rewardGiven)
            {
                await PlayerCmd.GainGold(200m, Owner);
            }

            if (result == 4)
            {
                await CreatureCmd.GainMaxHp(Owner.Creature, 25m);
            }

            if (result == 5)
            {
                IEnumerable<PotionModel> items = from p in base.Owner.Character.PotionPool.GetUnlockedPotions(base.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(base.Owner.UnlockState))
                                                 where p.Rarity == PotionRarity.Rare || p.Rarity == PotionRarity.Event
                                                 select p;
                PotionModel potionModel = base.Owner.PlayerRng.Rewards.NextItem(items);
                PotionModel potionModel2 = base.Owner.PlayerRng.Rewards.NextItem(items);
                List<Reward> rewards = new List<Reward>();
                if (potionModel != null)
                    rewards.Add(new PotionReward(potionModel.ToMutable(), base.Owner));
                if (potionModel2 != null)
                    rewards.Add(new PotionReward(potionModel2.ToMutable(), base.Owner));
                if (rewards.Count > 0)
                {
                    await RewardsCmd.OfferCustom(base.Owner, rewards);
                }
            }

            if (result == 6)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                ForNonCombatWithDefaultOdds([Owner!.Character.CardPool]), 10, Owner)]);
            }

            if (result == 7)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterRareCards), 5, Owner)]);
            }

            if (result == 8)
            {
                List<CardModel> cards = (await CardSelectCmd.FromDeckForRemoval(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, 2), player: base.Owner)).ToList();
                if (cards.Count > 0)
                {
                    await CardPileCmd.RemoveFromDeck(cards);
                    NDebugAudioManager.Instance?.Play("card_smith.mp3", 1f, PitchVariance.Small);
                    NGame.Instance.ScreenShakeTrauma(ShakeStrength.Strong);
                }
                else
                {
                    await PlayerCmd.GainGold(200m, Owner);
                }
            }
        }

        protected async Task RareRewards()
        {
            int result = MiyabiFuncBase.RandomInt(0, 9, Owner);
            bool rewardGiven = false;

            if (result == 0)
            {
                int enchantResult = MiyabiFuncBase.RandomInt(0, 5, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Corrupted>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 1 && !rewardGiven)
                {
                    if (await TryEnchantCard<Imbued>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 2 && !rewardGiven)
                {
                    if (await TryEnchantCard< RoyallyApproved>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 3 && !rewardGiven)
                {
                    if (await TryEnchantCard<Spiral>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 4 && !rewardGiven)
                {
                    if (await TryEnchantCard<Clone>())
                    {
                        rewardGiven = true;
                    }
                }
            }

            if (result == 1)
            {
                var relic = new RelicReward(RelicRarity.Rare | RelicRarity.Event, Owner);
                if (relic != null && relic.Relic is not Circlet)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    rewardGiven = true;
                }
            }

            if (result == 2)
            {
                List<CardModel> cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2))).ToList();
                if (cardModel.Count > 0)
                {
                    foreach (var card in cardModel)
                        CardCmd.Upgrade(card);
                    rewardGiven = true;
                }
            }

            if (result <= 3 && !rewardGiven)
            {
                await PlayerCmd.GainGold(100m, Owner);
            }

            if (result == 4)
            {
                await CreatureCmd.GainMaxHp(Owner.Creature, 15m);
            }

            if (result == 5)
            {
                IEnumerable<PotionModel> items = from p in base.Owner.Character.PotionPool.GetUnlockedPotions(base.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(base.Owner.UnlockState))
                                                 where p.Rarity == PotionRarity.Rare || p.Rarity == PotionRarity.Event
                                                 select p;
                PotionModel potionModel = base.Owner.PlayerRng.Rewards.NextItem(items);
                if (potionModel != null)
                {
                    await RewardsCmd.OfferCustom(base.Owner, new List<Reward>(1)
                    {
                        new PotionReward(potionModel.ToMutable(), base.Owner)
                    });
                }
            }

            if (result == 6)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                    ForNonCombatWithDefaultOdds([Owner!.Character.CardPool]), 6, Owner)]);
            }

            if (result == 7)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterRareCards), 3, Owner)]);
            }

            if (result == 8)
            {
                List<CardModel> cards = (await CardSelectCmd.FromDeckForRemoval(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, 1), player: base.Owner)).ToList();
                if (cards.Count > 0)
                {
                    await CardPileCmd.RemoveFromDeck(cards);
                    NDebugAudioManager.Instance?.Play("card_smith.mp3", 1f, PitchVariance.Small);
                    NGame.Instance.ScreenShakeTrauma(ShakeStrength.Strong);
                }
                else
                {
                    await PlayerCmd.GainGold(100m, Owner);
                }
            }
        }

        protected async Task UncommonRewards()
        {
            int result = MiyabiFuncBase.RandomInt(0, 9, Owner);
            bool rewardGiven = false;

            if (result == 0)
            {
                int enchantResult = MiyabiFuncBase.RandomInt(0, 10, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<SoulsPower>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 1 && !rewardGiven)
                {
                    if (await TryEnchantCard<Inky>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 2 && !rewardGiven)
                {
                    if (await TryEnchantCard<Momentum>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 3 && !rewardGiven)
                {
                    if (await TryEnchantCard<PerfectFit>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 4 && !rewardGiven)
                {
                    if (await TryEnchantCard<Glam>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 5 && !rewardGiven)
                {
                    if (await TryEnchantCard<Adroit>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 6 && !rewardGiven)
                {
                    if (await TryEnchantCard<Sown>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 7 && !rewardGiven)
                {
                    if (await TryEnchantCard<Steady>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 8 && !rewardGiven)
                {
                    if (await TryEnchantCard<Vigorous>())
                    {
                        rewardGiven = true;
                    }
                }
                if(enchantResult <= 9 && !rewardGiven)
                {
                    if (await TryEnchantCard<SlumberingEssence>())
                    {
                        rewardGiven = true;
                    }
                }
            }

            if(result == 1)
            {
                var relic = new RelicReward(RelicRarity.Uncommon, Owner);
                if (relic != null && relic.Relic is not Circlet)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    rewardGiven = true;
                }
            }

            if (result == 2)
            {
                CardModel cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1))).FirstOrDefault();
                if (cardModel != null)
                {
                    CardCmd.Upgrade(cardModel);
                    rewardGiven = true;
                }
            }

            if (result <= 3 && !rewardGiven)
            {
                await PlayerCmd.GainGold(60m, Owner);
            }

            if (result == 4)
            {
                if (Owner.Creature.CurrentHp < Owner.Creature.MaxHp / 2)
                {
                    await CreatureCmd.Heal(Owner.Creature, 25m);
                }
                else
                {
                    await CreatureCmd.GainMaxHp(Owner.Creature, 9m);
                }
            }

            if (result == 5)
            {
                IEnumerable<PotionModel> items = from p in base.Owner.Character.PotionPool.GetUnlockedPotions(base.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(base.Owner.UnlockState))
                                                 where p.Rarity == PotionRarity.Uncommon
                                                 select p;
                PotionModel potionModel = base.Owner.PlayerRng.Rewards.NextItem(items);
                if (potionModel != null)
                {
                    await RewardsCmd.OfferCustom(base.Owner, new List<Reward>(1)
            {
                new PotionReward(potionModel.ToMutable(), base.Owner)
            });
                }
            }

            if( result == 6)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                    ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterUnandCCards), 5, Owner)]);
            }

            if(result == 7)
            {
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterUncommonCards), 3, Owner)]);
            }

            if (result == 8)
            {
                CardModel cardModel = (await CardSelectCmd.FromDeckForTransformation(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 1))).FirstOrDefault();
                if (cardModel != null)
                {
                    await CardCmd.TransformToRandom(cardModel, base.Owner.PlayerRng.Rewards, CardPreviewStyle.EventLayout);
                }
                else
                {
                    await PlayerCmd.GainGold(60m, Owner);
                }
            }
        }

        protected async Task CommonRewards()
        {
            int result = MiyabiFuncBase.RandomInt(0, 7, Owner);
            bool rewardGiven = false;

            if (result == 5)
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                    ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterCommonCards), 3, Owner)]);

            if(result == 0)
            {
                int enchantResult = MiyabiFuncBase.RandomInt(0, 5, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Nimble>(2m))
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 1 && !rewardGiven)
                {
                    if (await TryEnchantCard<Sharp>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 2 && !rewardGiven)
                {
                    if (await TryEnchantCard<Swift>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 3 && !rewardGiven)
                {
                    if (await TryEnchantCard<Goopy>())
                    {
                        rewardGiven = true;
                    }
                }
                if (enchantResult <= 4 && !rewardGiven)
                {
                    if (await TryEnchantCard<Slither>())
                    {
                        rewardGiven = true;
                    }
                }
            }

            if(result == 1)
            {
                var relic = new RelicReward(RelicRarity.Common, Owner);
                if(relic != null && relic.Relic is not Circlet)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    rewardGiven = true;
                }
            }

            if (result == 2)
            {
                IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(base.Owner.RunState.Rng.Niche)
                    .Take(1);
                foreach (CardModel item in enumerable)
                {
                    CardCmd.Upgrade(item);
                    rewardGiven = true;
                }
            }

            if (result <= 3 && !rewardGiven)
            {
                await PlayerCmd.GainGold(30m, Owner);
            }

            if (result == 4)
            {
                if(Owner.Creature.CurrentHp < Owner.Creature.MaxHp / 2)
                {
                    await CreatureCmd.Heal(Owner.Creature, 12m);
                }
                else
                {
                    await CreatureCmd.GainMaxHp(Owner.Creature, 4m);
                }
            }

            if(result == 6)
            {
                IEnumerable<PotionModel> items = from p in base.Owner.Character.PotionPool.GetUnlockedPotions(base.Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(base.Owner.UnlockState))
                                                 where p.Rarity == PotionRarity.Common
                                                 select p;
                PotionModel potionModel = base.Owner.PlayerRng.Rewards.NextItem(items);
                if (potionModel != null)
                {
                    await RewardsCmd.OfferCustom(base.Owner, new List<Reward>(1)
                    {
                        new PotionReward(potionModel.ToMutable(), base.Owner)
                    });
                }
            }
        }


        protected bool FilterRareCards(CardModel card)
        {
            return card.Rarity == CardRarity.Rare;
        }
        protected bool FilterUncommonCards(CardModel card)
        {
            return card.Rarity == CardRarity.Uncommon;
        }
        protected bool FilterCommonCards(CardModel card)
        {
            return card.Rarity == CardRarity.Common;
        }
        protected bool FilterUnandCCards(CardModel card)
        {
            return card.Rarity == CardRarity.Common || card.Rarity == CardRarity.Uncommon;
        }

        protected async Task<bool> TryEnchantCard<T>(decimal amount = 1m) where T : EnchantmentModel
        {
            CardModel cardModel = (await CardSelectCmd.FromDeckForEnchantment(base.Owner, ModelDb.Enchantment<T>(), (int)amount, new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt,1))).FirstOrDefault();
            if (cardModel != null)
            {
                CardCmd.Enchant<T>(cardModel, amount);
                NCardEnchantVfx nCardEnchantVfx = NCardEnchantVfx.Create(cardModel);
                if (nCardEnchantVfx != null)
                {
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
                }
                return true;
            }
            return false;
        }


    }


    public class ChoukaRestSiteOption : ModRestSiteOptionTemplate
    {
        public static int Cost => 60;

        //public override LocString Description
        //{
        //    get
        //    {
        //        LocString description = base.Description;
        //        ChoukaRelic relic = base.Owner.GetRelic<ChoukaRelic>();
        //        //relic.AddCounter(1);
        //        return description;
        //    }
        //}

        public override string OptionId => "CHOU_KA";

        public override string? CustomIconPath => "res://images/elseui/option_chouka.png";


        public ChoukaRestSiteOption(Player owner)
            : base(owner)
        {
        }

        public override async Task<bool> OnSelect()
        {
            await base.Owner.GetRelic<ChoukaRelic>().OnUsed();
            return false;
        }

        public override Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
        {
            NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Normal);
            return Task.CompletedTask;
        }

        public override Task DoRemotePostSelectVfx()
        {
            NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == base.Owner);
            nRestSiteCharacter?.Shake();
            NRelicFlashVfx nRelicFlashVfx = NRelicFlashVfx.Create(ModelDb.Relic<ChoukaRelic>());
            if (nRelicFlashVfx == null)
            {
                return Task.CompletedTask;
            }
            nRestSiteCharacter?.AddChildSafely(nRelicFlashVfx);
            nRelicFlashVfx.Position = Vector2.Zero;
            return Task.CompletedTask;
        }
    }
}
