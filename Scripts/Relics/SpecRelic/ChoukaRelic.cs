using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
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
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics.SpecRelic
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class ChoukaRelic : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/relics/choukaRelic.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        private int _counter = 0;

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

        public void AddCounter(int amount)
        {
            this.Counter += amount;
            //this.Flash(); // 让遗物闪烁一下，视觉效果更好
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CINIMA",0),
            new DynamicVar("Uppercount", 50m)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                if (DynamicVars["CINIMA"].BaseValue >= 1)
                {
                    await PowerCmd.Apply<XsjsPower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }

                if (DynamicVars["CINIMA"].BaseValue >= 2)
                {
                    await PowerCmd.Apply<TunafaPower>(choiceContext, base.Owner.Creature, 3m, null, null);
                }

                if (DynamicVars["CINIMA"].BaseValue >= 3)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 4m, null, null);
                }

                if (DynamicVars["CINIMA"].BaseValue >= 4)
                {
                    await PowerCmd.Apply<JunLiePower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }

                if (DynamicVars["CINIMA"].BaseValue >= 5)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 6m, null, null);
                }

                if (DynamicVars["CINIMA"].BaseValue >= 6)
                {
                    await PowerCmd.Apply<TianzizyPower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }
            }
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

        private bool _hasDone = false;
        private bool _isBusy = false;
        private int _lastCinima = 0;
        public async Task OnUsed()
        {
            if(Owner.Gold < ChoukaRestSiteOption.Cost)
            {
                return;
            }
            if (!_isBusy)
            {
                try
                {
                    _isBusy = true;

                    Flash();
                    AddCounter(1);
                    Owner.Gold -= ChoukaRestSiteOption.Cost;

                    if (DynamicVars["CINIMA"].BaseValue < 6 && Counter - _lastCinima >= 80)
                    {
                        DynamicVars["CINIMA"].BaseValue += 1;
                        _lastCinima = Counter;
                        _hasDone = true;
                    }

                    int result = MiyabiFuncBase.RadomInt(0, 50, Owner);
                    if (DynamicVars["CINIMA"].BaseValue < 6 && !_hasDone)
                    {
                        if (result == 0)
                        {
                            DynamicVars["CINIMA"].BaseValue += 1;
                            _hasDone = true;
                        }
                    }
                    if (result <= 4 && !_hasDone)
                    {
                        await AnicientRewards();
                        _hasDone = true;
                    }
                    if (result <= 9 && !_hasDone)
                    {
                        await RareRewards();
                        _hasDone = true;
                    }
                    if (result <= 24 && !_hasDone)
                    {
                        await UncommonRewards();
                        _hasDone = true;
                    }
                    if (!_hasDone)
                    {
                        await CommonRewards();
                        _hasDone = true;
                    }

                    _hasDone = false;

                }
                finally
                {
                    _isBusy = false;
                }
            }
            
        }

        private async Task AnicientRewards()
        {
            int result = MiyabiFuncBase.RadomInt(0, 9, Owner);
            bool _hasDone = false;

            if (result == 0)
            {
                int enchantResult = MiyabiFuncBase.RadomInt(0, 2, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Instinct>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 1 && !_hasDone)
                {
                    if (await TryEnchantCard< TezcatarasEmber>())
                    {
                        _hasDone = true;
                    }
                }
            }

            if (result == 1)
            {
                var relic = new RelicReward(RelicRarity.Ancient | RelicRarity.Shop, Owner);
                if (relic != null)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    _hasDone = true;
                }
            }

            if (result == 2)
            {
                CardModel cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 0, 2))).FirstOrDefault();
                if (cardModel != null)
                {
                    CardCmd.Upgrade(cardModel);
                    _hasDone = true;
                }
                _hasDone = true;
            }

            if (result <= 3 && !_hasDone)
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
                if (potionModel != null || potionModel2 != null)
                {
                    await RewardsCmd.OfferCustom(base.Owner, new List<Reward>(1)
                    {
                        new PotionReward(potionModel.ToMutable(), base.Owner),
                        new PotionReward(potionModel2.ToMutable(), base.Owner)
                    });
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
                await CardPileCmd.RemoveFromDeck(cards);
                NDebugAudioManager.Instance?.Play("card_smith.mp3", 1f, PitchVariance.Small);
                NGame.Instance.ScreenShakeTrauma(ShakeStrength.Strong);
            }
        }

        private async Task RareRewards()
        {
            int result = MiyabiFuncBase.RadomInt(0, 9, Owner);
            bool _hasDone = false;

            if (result == 0)
            {
                int enchantResult = MiyabiFuncBase.RadomInt(0, 5, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Corrupted>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 1 && !_hasDone)
                {
                    if (await TryEnchantCard<Imbued>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 2 && !_hasDone)
                {
                    if (await TryEnchantCard< RoyallyApproved>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 3 && !_hasDone)
                {
                    if (await TryEnchantCard<Spiral>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 4 && !_hasDone)
                {
                    if (await TryEnchantCard<Clone>())
                    {
                        _hasDone = true;
                    }
                }
            }

            if (result == 1)
            {
                var relic = new RelicReward(RelicRarity.Rare | RelicRarity.Event, Owner);
                if (relic != null)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    _hasDone = true;
                }
            }

            if (result == 2)
            {
                CardModel cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 0, 2))).FirstOrDefault();
                if (cardModel != null)
                {
                    CardCmd.Upgrade(cardModel);
                    _hasDone = true;
                }
                _hasDone = true;
            }

            if (result <= 3 && !_hasDone)
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
                await CardPileCmd.RemoveFromDeck(cards);
                NDebugAudioManager.Instance?.Play("card_smith.mp3", 1f, PitchVariance.Small);
                NGame.Instance.ScreenShakeTrauma(ShakeStrength.Strong);
            }
        }

        private async Task UncommonRewards()
        {
            int result = MiyabiFuncBase.RadomInt(0, 9, Owner);
            bool _hasDone = false;

            if (result == 0) 
            {
                int enchantResult = MiyabiFuncBase.RadomInt(0, 10, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<SoulsPower>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 1 && !_hasDone)
                {
                    if (await TryEnchantCard<Inky>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 2 && !_hasDone)
                {
                    if (await TryEnchantCard<Momentum>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 3 && !_hasDone)
                {
                    if (await TryEnchantCard<PerfectFit>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 4 && !_hasDone)
                {
                    if (await TryEnchantCard<Glam>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 5 && !_hasDone)
                {
                    if (await TryEnchantCard<Adroit>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 6 && !_hasDone)
                {
                    if (await TryEnchantCard<Sown>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 7 && !_hasDone)
                {
                    if (await TryEnchantCard<Steady>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 8 && !_hasDone)
                {
                    if (await TryEnchantCard<Vigorous>())
                    {
                        _hasDone = true;
                    }
                }
                if(enchantResult <= 9 && !_hasDone)
                {
                    if (await TryEnchantCard<SlumberingEssence>())
                    {
                        _hasDone = true;
                    }
                }
            }

            if(result == 1)
            {
                var relic = new RelicReward(RelicRarity.Uncommon, Owner);
                if (relic != null)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    _hasDone = true;
                }
            }

            if (result == 2)
            {
                CardModel cardModel = (await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 0, 1))).FirstOrDefault();
                if (cardModel != null)
                {
                    CardCmd.Upgrade(cardModel);
                    _hasDone = true;
                }
            }

            if (result <= 3 && !_hasDone)
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
            }
        }

        private async Task CommonRewards()
        {
            int result = MiyabiFuncBase.RadomInt(0, 7, Owner);
            bool _hasDone = false;

            if (result == 5)
                await RewardsCmd.OfferCustom(Owner!, [new CardReward(CardCreationOptions.
                    ForNonCombatWithDefaultOdds([Owner!.Character.CardPool], FilterCommonCards), 3, Owner)]);

            if(result == 0)
            {
                int enchantResult = MiyabiFuncBase.RadomInt(0, 5, Owner);
                if (enchantResult == 0)
                {
                    if (await TryEnchantCard<Nimble>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 1 && !_hasDone)
                {
                    if (await TryEnchantCard<Sharp>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 2 && !_hasDone)
                {
                    if (await TryEnchantCard<Swift>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 3 && !_hasDone)
                {
                    if (await TryEnchantCard<Goopy>())
                    {
                        _hasDone = true;
                    }
                }
                if (enchantResult <= 4 && !_hasDone)
                {
                    if (await TryEnchantCard<Slither>())
                    {
                        _hasDone = true;
                    }
                }
            }
            
            if(result == 1)
            {
                var relic = new RelicReward(RelicRarity.Common, Owner);
                if(relic != null)
                {
                    await RewardsCmd.OfferCustom(Owner!, [relic]);
                    _hasDone = true;
                }
            }

            if (result == 2)
            {
                IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(base.Owner.RunState.Rng.Niche)
                    .Take(1);
                foreach (CardModel item in enumerable)
                {
                    CardCmd.Upgrade(item);
                    _hasDone = true;
                }
            }

            if (result <= 3 && !_hasDone)
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


        private bool FilterRareCards(CardModel card)
        {
            return card.Rarity == CardRarity.Rare;
        }
        private bool FilterUncommonCards(CardModel card)
        {
            return card.Rarity == CardRarity.Uncommon;
        }
        private bool FilterCommonCards(CardModel card)
        {
            return card.Rarity == CardRarity.Common;
        }
        private bool FilterUnandCCards(CardModel card)
        {
            return card.Rarity == CardRarity.Common || card.Rarity == CardRarity.Uncommon;
        }

        private async Task<bool> TryEnchantCard<T>() where T : EnchantmentModel
        {
            CardModel cardModel = (await CardSelectCmd.FromDeckForEnchantment(base.Owner, ModelDb.Enchantment<T>(), 1, new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt,0, 1))).FirstOrDefault();
            if (cardModel != null)
            {
                CardCmd.Enchant<T>(cardModel, 1m);
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


    public class ChoukaRestSiteOption : CustomRestSiteOption
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
            await Task.FromResult(result: true);
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
