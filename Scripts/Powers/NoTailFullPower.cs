using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class NoTailFullPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
            HoverTipFactory.FromKeyword(MiyabiKeywords.OtherWorldFriends),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
        ];

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            IEnumerable<CardModel> allCards = base.Owner.Player.PlayerCombatState.AllCards;
            foreach (CardModel item in allCards)
            {
                if (item.Type != CardType.Attack
                    || item.CanonicalKeywords.Contains(MiyabiKeywords.Friends) || item.CanonicalKeywords.Contains(MiyabiKeywords.OtherWorldFriends))
                    continue;
                item.AddKeyword(CardKeyword.Exhaust);
                item.AddKeyword(CardKeyword.Ethereal);
                item.SetToFreeThisCombat();
                if (item.IsUpgradable)
                    item.UpgradeInternal();
            }
        }

        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Type != CardType.Attack
                    || card.CanonicalKeywords.Contains(MiyabiKeywords.Friends) || card.CanonicalKeywords.Contains(MiyabiKeywords.OtherWorldFriends))
                return;
            card.AddKeyword(CardKeyword.Exhaust);
            card.AddKeyword(CardKeyword.Ethereal);
            card.SetToFreeThisCombat();
            if (card.IsUpgradable)
                card.UpgradeInternal();
        }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (card.Owner.Creature != base.Owner || card.Type != CardType.Attack || card.CanonicalKeywords.Contains(MiyabiKeywords.Friends) || card.CanonicalKeywords.Contains(MiyabiKeywords.OtherWorldFriends))
            {
                return false;
            }
            // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }

        // 核心逻辑 2：修改星能/特殊消耗（保持逻辑一致）
        public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (card.Owner.Creature != base.Owner || card.Type != CardType.Attack || card.CanonicalKeywords.Contains(MiyabiKeywords.Friends) || card.CanonicalKeywords.Contains(MiyabiKeywords.OtherWorldFriends))
            {
                return false;
            }
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player || !cardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang)) return;

            Flash();

            await CreatureCmd.Damage(choiceContext, base.Owner, 1, ValueProp.Unpowered, null, null);
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 1, null, null);
        }
    }
}
