using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TrainSpeakOverPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        int playCount = 5;

        // 定义内部数据类，用于记录本回合已打出的卡牌数
        private class Data
        {
            public int cardsPlayedThisTurn;
        }

        protected override object InitInternalData()
        {
            return new Data();
        }

        // 核心逻辑 1：修改能量消耗
        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (ShouldSkip(card))
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
            if (ShouldSkip(card))
            {
                return false;
            }
            modifiedCost = Math.Max(0m, originalCost - 1m);
            return true;
        }

        // 计数逻辑：打出卡牌后增加计数
        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            // 只有玩家手动打出的、属于该 Power 拥有者的卡才计数
            if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay != null && !cardPlay.IsAutoPlay && cardPlay.IsLastInSeries)
            {
                GetInternalData<Data>().cardsPlayedThisTurn++;
            }
            return Task.CompletedTask;
        }

        // 回合开始重置计数
        public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if (side == base.Owner.Side)
            {
                GetInternalData<Data>().cardsPlayedThisTurn = 0;
            }
            return Task.CompletedTask;
        }

        // 判定条件：是否应该跳过减费效果
        private bool ShouldSkip(CardModel card)
        {
            // 1. 如果卡牌拥有者不是该 Power 拥有者，跳过
            if (card.Owner.Creature != base.Owner) return true;

            // 2. 只有手牌中的卡显示减费效果
            bool inHand = card.Pile?.Type == PileType.Hand || card.Pile?.Type == PileType.Play;
            if (!inHand) return true;

            return GetInternalData<Data>().cardsPlayedThisTurn >= playCount;
        }
    }
}
