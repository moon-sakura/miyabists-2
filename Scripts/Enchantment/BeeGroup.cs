using BaseLib.Abstracts;
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
    internal class BeeGroupEnchantment : CustomEnchantmentModel
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


        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

        // 图标位置。大小1:1就行，原版是64x64
        protected override string? CustomIconPath => "res://images/enchant/beeGroup.png";

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
            await base.OnPlay(choiceContext, cardPlay);

            List<CardModel> cards = [];
            cards.AddRange(base.Card.Owner.PlayerCombatState.DrawPile.Cards.ToList());
            cards.AddRange(base.Card.Owner.PlayerCombatState.Hand.Cards.ToList());
            foreach (var cardin in cards) 
            {
                if (!CanEnchant(cardin))
                {
                    cards.Remove(cardin);
                }
            }

            var card = cards.TakeRandom(1, base.Card.Owner.RunState.Rng.Shuffle).FirstOrDefault();

            if(card != null)
                CardCmd.Enchant<BeeGroupEnchantment>(card,1m).SetTemporary(true);
        }


        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if(base.Card.Pile == null) { return; }
            if (base.Card.Pile.Type != PileType.Hand || base.Card.Pile.Type != PileType.Draw)
            {
                return;
            }

            if (MiyabiFuncBase.GetIsTrue100(10, base.Card.Owner))
            {
                await CardCmd.AutoPlay(choiceContext,base.Card,null); return;
            }
        }

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            if(isTemporary)
                CardCmd.ClearEnchantment(base.Card);
        }
    }
}
