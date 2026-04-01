using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostFallPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        //public override bool AllowNegative => true;

        public override int DisplayAmount => Amount - 1;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<ShuangYue>()
        ];


        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private class Data
        {
            public readonly HashSet<Creature> casters = new HashSet<Creature>();

            public readonly Dictionary<CardModel, int> downgradedCardsToOldUpgradeLevels = new Dictionary<CardModel, int>();
        }
        protected override object InitInternalData()
        {
            return new Data();
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            await CheckFallPower();
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckFallPower();
        }

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            if (side == base.Owner.Side)
            {
                await CheckFallPower();
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                IEnumerable<CardModel> enumerable = base.Owner.Player.PlayerCombatState.AllCards.Where((CardModel c) => c.IsUpgraded && c is ShuangYue);
                foreach (CardModel item in enumerable)
                {
                    GetInternalData<Data>().downgradedCardsToOldUpgradeLevels.Add(item, item.CurrentUpgradeLevel);
                    CardCmd.Downgrade(item);
                }
            }
        }

        private async Task CheckFallPower()
        {
            if (CardPile.GetCards(base.Owner.Player, PileType.Hand).ToList().Count() < CardPile.maxCardsInHand && GetCards().ToList().Count() == 0 && DisplayAmount >= 2)
            {
                // 加入一张《霜月》到手中
                CardModel reward1 = base.Owner.CombatState.CreateCard<ShuangYue>(base.Owner.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);

                // 如果你的逻辑是触发后消耗层数，可以加在这里。
                // 如果只是“大于 2 点就给一张”且不消耗，则不写 Reduce。
                //await AmountChange(2);
                await PowerCmd.Apply<FrostFallPower>(base.Owner, -2, null, null);
                //base.Owner.Player.GetRelic<SwordNotailRelic>().LuoShuangCostThisTurn -= 2;
            }
        }

        private IEnumerable<CardModel> GetCards()
        {
            CardPile pile = PileType.Hand.GetPile(base.Owner.Player);
            return pile.Cards.Where((CardModel c) => c is ShuangYue);
        }

    }
}
