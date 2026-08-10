using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Enchantment
{
    internal class BeeGroupEnchantment : ModEnchantmentTemplate
    {
        // 是否在卡牌上显示数值
        public override bool ShowAmount => false;

        // 重载这个以改变显示的数字
        // public override int DisplayAmount => DynamicVars.Cards.IntValue;

        // 是否会添加额外的卡牌描述文本
        public override bool HasExtraCardText => true;

        [SavedProperty]
        public bool isTemporary { get; private set; } = false;

        public void SetTemporary(bool temporary)
        {
            isTemporary = temporary;
        }



        //[SavedProperty]
        //public int TriggerChance { get; private set; } = 1;

        //public void SetChance(int chance)
        //{
        //    TriggerChance = chance;
        //    DynamicVars["Chance"].BaseValue = TriggerChance;
        //    DynamicVars["OriChance"].BaseValue = TriggerChance;
        //}


        //protected override IEnumerable<DynamicVar> CanonicalVars => 
        //[
        //    new DynamicVar("Chance",1),
        //    new DynamicVar("OriChance",1),
        //];
        //protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

        // 图标位置。大小1:1就行，原版是64x64
        public override string? CustomIconPath => "res://images/enchant/beeGroup.png";

        // 决定是否可以附魔到某张卡牌上
        public override bool CanEnchant(CardModel card)
        {
            return base.CanEnchant(card);
        }

        // 当附魔被应用时调用
        protected override void OnEnchant()
        {
            base.OnEnchant();
        }

        // 当附魔的卡牌被打出时调用。
        public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
        {
            List<CardModel> cards = [];
            cards.AddRange(base.Card.Owner.PlayerCombatState.DrawPile.Cards.ToList());
            cards.AddRange(base.Card.Owner.PlayerCombatState.Hand.Cards.ToList());

            List<CardModel> validCards = cards.Where(cardin => CanEnchant(cardin)).ToList();
            var targetCard = validCards.TakeRandom(1, base.Card.Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();

            if (targetCard != null)
            {
                var e = CardCmd.Enchant<BeeGroupEnchantment>(targetCard, 1m);
                e.SetTemporary(true);
                //e.SetChance(DynamicVars["OriChance"].IntValue);
            }
        }


        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if(cardPlay.Card.Owner != base.Card.Owner) { return; }
            if (base.Card.Pile == null) { return; }
            if (base.Card.Pile.Type != PileType.Hand && base.Card.Pile.Type != PileType.Draw)
            {
                return;
            }

            if (cardPlay.IsAutoPlay)
            {
                return;
            }

            if (MiyabiFuncBase.GetIsTrue100(20, base.Card.Owner)||(cardPlay.Card.Enchantment is BeeGroupEnchantment && MiyabiFuncBase.GetIsTrue100(35, base.Card.Owner)))
            {
                await CardCmd.AutoPlay(choiceContext, base.Card, null);
                //TriggerChance--;
                //DynamicVars["Chance"].BaseValue = TriggerChance;
                //if (TriggerChance <= 0)
                //{
                //    CardCmd.ClearEnchantment(base.Card);
                //}
            }
        }

        //public override async Task AfterCombatEnd(CombatRoom room)
        //{
        //    if(isTemporary)
        //        CardCmd.ClearEnchantment(base.Card);
        //}
    }
}
