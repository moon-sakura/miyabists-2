using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Enchantment;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class BeeGroup : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/beeGroup2.png";
        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.OtherWorldFriends];

        public BeeGroup()
            : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(1),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromEnchantment<BeeGroupEnchantment>().FirstOrDefault(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            List<CardModel> cards = [];
            cards.AddRange(base.Owner.PlayerCombatState.DrawPile.Cards.ToList());
            cards.AddRange(base.Owner.PlayerCombatState.Hand.Cards.ToList());
            var beeEnchant = ModelDb.Enchantment<BeeGroupEnchantment>();

            List<CardModel> validCards = cards.Where(cardin => beeEnchant.CanEnchant(cardin)).ToList();
            var targetCard = validCards.TakeRandom(DynamicVars.Cards.IntValue, base.Owner.RunState.Rng.Shuffle);

            if (targetCard != null)
            {
                foreach (var card in targetCard)
                {CardCmd.Enchant<BeeGroupEnchantment>(card, 1m)?.SetTemporary(true); }
               
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
            //DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
