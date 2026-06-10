using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Relics;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class ShannengPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        private const int MaxPoints = 151; // 上限为 150

        public override int DisplayAmount => Amount - 1;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/shanneng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Used",0),
        ];

        /// <summary>本场战斗累计消耗的闪能总量（供青溟震击等卡牌使用）</summary>
        public int TotalConsumed { get; private set; } = 0;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (base.Amount > MaxPoints)
            {
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                SetAmount(1);
            }
        }

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power != this) return;
            if (base.Amount > MaxPoints)
            {
                SetAmount(MaxPoints);
            }
            else if (base.Amount < 1)
            {
                SetAmount(1);
            }
        }


        public bool CanUseShanneng(int a)
        {
            if (a <= DisplayAmount) return true;
            return false;
        }

        public async Task UseShanneng(PlayerChoiceContext choiceContext, int a)
        {
            if (!CanUseShanneng(a)) return;

            // 使用后减少闪能点数
            SetAmount(base.Amount - a);
            DynamicVars["Used"].BaseValue += a;
            TotalConsumed += a;

            if (DynamicVars["Used"].BaseValue > MaxPoints - 1)
            {
                CardModel reward1 = base.Owner.CombatState.CreateCard<FufaQianchong>(base.Owner.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner.Player, CardPilePosition.Random);
                TotalConsumed = 0;
                DynamicVars["Used"].BaseValue = 0;
            }

            var qingmingShufaRelic = (QingmingShufaRelic)MiyabiFuncBase.GetRelic<QingmingShufaRelic>(Owner.Player);
            var qingmingNiaoRelic = (QingmingNiaoRelic)MiyabiFuncBase.GetRelic<QingmingNiaoRelic>(Owner.Player);

            if (qingmingShufaRelic != null)
            {
                qingmingShufaRelic.SetUsed((int)DynamicVars["Used"].BaseValue);
            }
            if(qingmingNiaoRelic != null)
            {
                qingmingNiaoRelic.SetUsed((int)DynamicVars["Used"].BaseValue);
            }

            // 除祟一击：消耗闪能时自动打出（从任意牌堆）
            //await AutoPlayChusuiYiji(choiceContext);
        }

        /// <summary>消耗闪能后，自动打出除祟一击（无论在手牌、抽牌堆还是弃牌堆）</summary>
        //private async Task AutoPlayChusuiYiji(PlayerChoiceContext choiceContext)
        //{
        //    var player = Owner.Player;
        //    var allCards = player.PlayerCombatState.Hand.Cards.ToList();
        //    allCards.AddRange(player.PlayerCombatState.DrawPile.Cards);
        //    allCards.AddRange(player.PlayerCombatState.DiscardPile.Cards);

        //    var chusuiCards = allCards.Where(c => c is ChusuiYiji).ToList();
        //    foreach (var card in chusuiCards)
        //    {
        //        if (card.Owner != player) continue;
        //        var target = Owner.CombatState.HittableEnemies.TakeRandom(1, player.RunState.Rng.CombatCardSelection).FirstOrDefault();
        //        if (target != null)
        //        {
        //            await CardCmd.AutoPlay(choiceContext, card, target);
        //        }
        //    }
        //}

        public void SetUsed(int used)
        {
            DynamicVars["Used"].BaseValue = used;
        }
    }
}
