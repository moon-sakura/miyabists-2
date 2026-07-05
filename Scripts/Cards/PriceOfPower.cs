using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts.Relics.SpecRelic;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(CurseCardPool))]
    internal class PriceOfPower : MiyabiCardBase
    {
        //protected override string ArtPath => "res://images/cards/priceOfPower.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Unplayable,
            CardKeyword.Innate,
            CardKeyword.Retain,
        ];

        public PriceOfPower()
            : base(-1, CardType.Power, CardRarity.Curse, TargetType.None)
        {
            //GD.Print($"[PriceOfPower] 构造函数被调用 — 实例Hash: {GetHashCode()}");
        }

        
        private bool _effect = false;

        [SavedProperty]
        public bool Effected
        {
            get => _effect;
            set
            {
                AssertMutable(); // 确保在合法的修改状态
                _effect = value;
            }
        }

        /// <summary>
        /// 卡牌被移出牌组时，如果之前触发过效果，则归还 ChoukaRelic 计数
        /// </summary>
        public override async Task BeforeCardRemoved(CardModel card)
        {
            GD.Print($"[PriceOfPower] BeforeCardRemoved — _effect={Effected}");

            if (card != this) return;

            if (Owner != null && Effected)
            {
                RemoveEffect();
                Effected = false;
            }
        }

        public override int MaxUpgradeLevel => 0;

        internal void RemoveEffect()
        {
            ChoukaRelic choukaRelic = (ChoukaRelic)MiyabiFuncBase.GetRelic<ChoukaRelic>(Owner);
            if (choukaRelic != null && choukaRelic.CinimaCounter > 0)
            {
                choukaRelic.AddCinimaCounter(-1);
                GD.Print($"[PriceOfPower] RemoveEffect — CinimaCounter -1 → {choukaRelic.CinimaCounter}");
            }
        }

        internal static void TryAddEffect(CardModel card)
        {
            if (card is not PriceOfPower priceOfPower) return;
            if (priceOfPower.Effected) return; // 已处理过，防止重复计数

            var owner = priceOfPower.Owner;
            if (owner == null) return;

            var choukaRelic = (ChoukaRelic)MiyabiFuncBase.GetRelic<ChoukaRelic>(owner);
            if (choukaRelic == null) return;
            if (choukaRelic.CinimaCounter >= 6) return;

            choukaRelic.AddCinimaCounter(1);
            priceOfPower.Effected = true;
            GD.Print($"[PriceOfPower] TryAddEffect — CinimaCounter +1 → {choukaRelic.CinimaCounter}");
        }
    }

    // ========== Harmony Patches ==========

    /// <summary>
    /// Patch 1：拦截 CardPileCmd.Add，PriceOfPower 被加入 Deck 时计数器 +1
    /// </summary>
    [HarmonyPatch]
    public static class PriceOfPowerAddToDeckPatch
    {
        [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add),
            typeof(CardModel), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool))]
        [HarmonyPostfix]
        public static void Postfix(CardModel card, PileType newPileType)
        {
            GD.Print($"[PP AddPatch] card={card.GetType().Name}, pile={newPileType}");
            if (newPileType != PileType.Deck) return;
            PriceOfPower.TryAddEffect(card);
        }
    }

    /// <summary>
    /// Patch 2：拦截 CardCmd.TransformToRandom
    ///   Prefix  — 原卡是 PriceOfPower → 计数器 -1
    ///   Postfix — 结果卡是 PriceOfPower → 计数器 +1
    /// </summary>
    [HarmonyPatch]
    public static class PriceOfPowerTransformPatch
    {
        [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.TransformToRandom),
            typeof(CardModel), typeof(Rng), typeof(CardPreviewStyle))]
        [HarmonyPrefix]
        public static void Prefix(CardModel original)
        {
            GD.Print($"[PP TransformPatch Prefix] original={original.GetType().Name}, isPP={original is PriceOfPower}");

            if (original is PriceOfPower pop && pop.Effected)
            {
                pop.RemoveEffect();
                pop.Effected = false; // 防止 BeforeCardRemoved 重复扣
                GD.Print($"[PP TransformPatch Prefix] 原卡是 PriceOfPower，已移除效果");
            }
        }

        //[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.TransformToRandom),
        //    typeof(CardModel), typeof(Rng), typeof(CardPreviewStyle))]
        //[HarmonyPostfix]
        //public static void Postfix(CardPileAddResult __result)
        //{
        //    GD.Print($"[PP TransformPatch Postfix] success={__result.success}, card={__result.cardAdded?.GetType().Name}");

        //    if (__result.success && __result.cardAdded is PriceOfPower)
        //    {
        //        PriceOfPower.TryAddEffect(__result.cardAdded);
        //    }
        //}
    }
}
