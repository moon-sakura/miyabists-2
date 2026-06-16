using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class JinlongSzbPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/jinlongSzb.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromKeyword(MiyabiKeywords.LieShuang)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if(player != Owner.Player)
            {
                return;
            }

            var handCards = Owner.Player.PlayerCombatState.Hand.Cards.ToList();
            if (handCards.Count == 0) return;

            CardSelectorPrefs prefs = new CardSelectorPrefs(
                CardSelectorPrefs.DiscardSelectionPrompt, 0, Amount);
            IEnumerable<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext, handCards, Owner.Player, prefs));

            if (selected.Count() == 0) return;

            await CardCmd.DiscardAndDraw(choiceContext, selected, selected.Count());
        }

        public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if(card.Owner == Owner.Player)
            {
                await CreatureCmd.Damage(choiceContext, Owner.CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
            }
        }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
        {
            if (card.Owner == Owner.Player && !causedByEthereal)
            {
                await CreatureCmd.Damage(choiceContext, Owner.CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
            }
        }
    }
}
