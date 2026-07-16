using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TunafaPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/tunafa.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CardCount", 0)
        ];


        // 定义内部数据类，用于记录本回合已打出的卡牌数
        private class Data
        {
            public int cardsPlayedThisTurn;
        }

        protected override object InitInternalData()
        {
            return new Data();
        }

        //public override int DisplayAmount => Amount - 1;


        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != base.Owner.Player || !(cardPlay.Card is FengHua)) return;

            //await PowerCmd.SetAmount<TunafaPower>(Owner, Amount + 1, null, null);

            if (cardPlay != null && cardPlay.IsLastInSeries)
            {
                GetInternalData<Data>().cardsPlayedThisTurn++;
                DynamicVars["CardCount"].BaseValue = GetInternalData<Data>().cardsPlayedThisTurn;
            }

            if(GetInternalData<Data>().cardsPlayedThisTurn <= Amount)
            {
                await PowerCmd.Apply<FrostFallPower>(context, Owner, Amount, Owner, null);
            }

            //if (DynamicVars["CardCount"].IntValue == 4) 
            //    Flash();

            //if(DynamicVars["CardCount"].IntValue >= 5) 
            //{
            //    await PowerCmd.Apply<FrostFallPower>(context, Owner, Amount, Owner, null);
            //    DynamicVars["CardCount"].BaseValue = 0;
            //}
        }



        // 核心逻辑 1：修改能量消耗
        //public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        //{
        //    modifiedCost = originalCost;
        //    if (ShouldSkip(card))
        //    {
        //        return false;
        //    }

        //    // 源码参考：这里不再设为 default(decimal)，而是减 1，且不能小于 0
        //    modifiedCost = 0;
        //    return true;
        //}

        // 回合开始重置计数
        public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side == base.Owner.Side)
            {
                GetInternalData<Data>().cardsPlayedThisTurn = 0;
            }
            return Task.CompletedTask;
        }

        // 判定条件：是否应该跳过减费效果
        //private bool ShouldSkip(CardModel card)
        //{
        //    // 1. 如果卡牌拥有者不是该 Power 拥有者，跳过
        //    if (card.Owner.Creature != base.Owner || !(card is FengHua)) return true;

        //    // 2. 只有手牌中的卡显示减费效果
        //    bool inHand = card.Pile?.Type == PileType.Hand || card.Pile?.Type == PileType.Play;
        //    if (!inHand) return true;

        //    return GetInternalData<Data>().cardsPlayedThisTurn >= Amount;
        //}
    }
}
