using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 失感 - 诅咒卡
    /// 随机降级一张卡牌（抽牌堆、手卡、弃牌堆）。
    /// 保留；不可升级。
    /// </summary>
    [RegisterCard(typeof(CurseCardPool))]
    internal class ShiGan : MiyabiCardBase
    {
        public ShiGan() : base(0, CardType.Curse, CardRarity.Curse, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_Guang/cards/common.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Retain,
            CardKeyword.Exhaust,
        ];

        public override int MaxUpgradeLevel => 0;

        protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 抽牌堆、手卡、弃牌堆中已升级的卡牌（没升级的卡降级不会有任何变化）
            List<CardModel> candidates = new[] { PileType.Draw, PileType.Hand, PileType.Discard }
                .SelectMany(pileType => pileType.GetPile(Owner).Cards)
                .Where(c => c.CurrentUpgradeLevel > 0)
                .ToList();

            if (candidates.Count == 0) return Task.CompletedTask;

            CardModel? target = candidates
                .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection)
                .FirstOrDefault();
            if (target == null) return Task.CompletedTask;

            CardCmd.Downgrade(target);

            return Task.CompletedTask;
        }
    }
}
