using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts.Relics.SpecRelic;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
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
            : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
        {
            //GD.Print($"[PriceOfPower] 构造函数被调用 — 实例Hash: {GetHashCode()}");
        }

        [SavedProperty]
        public bool _effect = false;

        /// <summary>
        /// 卡牌被移出牌组时，如果之前触发过效果，则归还 ChoukaRelic 计数
        /// </summary>
        public override async Task BeforeCardRemoved(CardModel card)
        {
            //GD.Print($"[PriceOfPower] BeforeCardRemoved 触发 — card={card.GetType().Name}, this={GetType().Name}, _effect={_effect}");

            if (card != this)
            {
                //GD.Print($"[PriceOfPower] BeforeCardRemoved — card != this, 跳过 (card hash: {card.GetHashCode()}, this hash: {GetHashCode()})");
                return;
            }
            if (!_effect)
            {
                //GD.Print($"[PriceOfPower] BeforeCardRemoved — _effect=false, 跳过");
                return;
            }

            //GD.Print($"[PriceOfPower] BeforeCardRemoved — _effect=true, Owner={Owner != null}");
            if (Owner != null)
            {
                ChoukaRelic choukaRelic = (ChoukaRelic)MiyabiFuncBase.GetRelic<ChoukaRelic>(Owner);
                //GD.Print($"[PriceOfPower] BeforeCardRemoved — ChoukaRelic={choukaRelic != null}, CinimaCounter={choukaRelic?.CinimaCounter}");
                if (choukaRelic != null && choukaRelic.CinimaCounter > 0)
                {
                    choukaRelic.AddCinimaCounter(-1);
                    //GD.Print($"[PriceOfPower] BeforeCardRemoved — CinimaCounter -1, 现在: {choukaRelic.CinimaCounter}");
                }
            }
        }

        public override int MaxUpgradeLevel => 0;
    }

    /// <summary>
    /// Harmony Patch：当 PriceOfPower 被添加到 Deck 时，增加 ChoukaRelic 的 CinimaCounter
    /// </summary>
    [HarmonyPatch]
    public static class PriceOfPowerAddToDeckPatch
    {
        // 实际签名: Add(CardModel card, PileType newPileType, CardPilePosition position=Bottom, AbstractModel? clonedBy=null, bool skipVisuals=false)
        [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add),
            typeof(CardModel), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool))]
        [HarmonyPostfix]
        public static void Postfix(CardModel card, PileType newPileType, CardPilePosition position, AbstractModel clonedBy, bool skipVisuals)
        {
            //GD.Print($"[PriceOfPower Patch] Postfix 触发 — cardType={card.GetType().Name}, pileType={newPileType}");

            if (card is not PriceOfPower priceOfPower)
            {
                //GD.Print($"[PriceOfPower Patch] card 不是 PriceOfPower (是 {card.GetType().Name}), 跳过");
                return;
            }

            //GD.Print($"[PriceOfPower Patch] card 是 PriceOfPower! Hash={priceOfPower.GetHashCode()}");

            if (newPileType != PileType.Deck)
            {
                //GD.Print($"[PriceOfPower Patch] pileType={newPileType} 不是 Deck, 跳过");
                return;
            }

            //GD.Print($"[PriceOfPower Patch] pileType=Deck, 继续检查 Owner...");

            var owner = priceOfPower.Owner;
            if (owner == null)
            {
                //GD.Print($"[PriceOfPower Patch] ⚠ Owner 为 null! 跳过");
                return;
            }

            //GD.Print($"[PriceOfPower Patch] Owner OK: {owner.Character?.GetType().Name}");

            var choukaRelic = (ChoukaRelic)MiyabiFuncBase.GetRelic<ChoukaRelic>(owner);
            if (choukaRelic == null)
            {
                //GD.Print($"[PriceOfPower Patch] ⚠ ChoukaRelic 未找到! 跳过");
                return;
            }

            //GD.Print($"[PriceOfPower Patch] ChoukaRelic 找到, CinimaCounter={choukaRelic.CinimaCounter}");

            if (choukaRelic.CinimaCounter >= 6)
            {
                //GD.Print($"[PriceOfPower Patch] CinimaCounter >= 6 ({choukaRelic.CinimaCounter}), 跳过");
                return;
            }

            choukaRelic.AddCinimaCounter(1);
            priceOfPower._effect = true;
            //GD.Print($"[PriceOfPower Patch] ✅ 成功! CinimaCounter +1 → {choukaRelic.CinimaCounter}, _effect=true");
        }
    }
}
