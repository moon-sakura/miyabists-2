using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Patches;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics.SpecRelic;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Relics
{
    /// <summary>
    /// 叶瞬光的抽卡遗物（影画）。
    /// 光进入明心镜会消耗所有正面能力，所以影画效果全部由遗物直接结算，不依赖能力。
    /// 影画等级由基类的 CinimaCounter 表示（0~6），每级解锁一条效果：
    ///   1. 第1回合开始时，获得6点青溟剑势，并将一张《照影》加入手卡。
    ///   2. 明心镜状态下，每次消耗青溟剑势，随机升级等量卡牌并获得1点能量。
    ///   3. 每回合开始时，获得2点力量。
    ///   4. 进入明心镜状态时，每种被消耗的能力额外为所有敌人施加10%失衡易伤。
    ///   5. 进入明心镜状态时，获得8点力量。
    ///   6. 战斗开始及之后每进入2次明心镜状态，为自己施加1层明灯愿。
    /// </summary>
    [RegisterRelic(typeof(YeshunGuangRelicPool))]
    internal class ChoukaRelicGuang : ChoukaRelic, ISharedType, IMingxinJingEnterListener
    {
        /// <summary>第 1 回合开始时获得的青溟剑势</summary>
        private const decimal FirstTurnJianshi = 6m;

        /// <summary>每回合开始时获得的力量</summary>
        private const decimal StrengthPerTurn = 2m;

        /// <summary>进入明心镜状态时获得的力量</summary>
        private const decimal StrengthOnMingxinEnter = 8m;

        /// <summary>每消耗 1 种能力额外施加的失衡易伤（%）</summary>
        private const decimal ExtraVulnerablePerPower = 10m;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
            HoverTipFactory.FromPower<MingDengYuanPower>(),
            HoverTipFactory.FromCard<ZhaoYing>(),
        ];

        private int _mingxinEnterCount;

        /// <summary>本场战斗进入明心镜状态的次数（用于影画 6，每 2 次触发一次）</summary>
        [SavedProperty]
        public int MingxinEnterCount
        {
            get => _mingxinEnterCount;
            private set
            {
                AssertMutable();
                _mingxinEnterCount = value;
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;

            DynamicVars["CINIMA"].BaseValue = CinimaCounter;

            if (Owner.Creature.CombatState is not { } combatState) return;

            if (combatState.RoundNumber == 1)
            {
                // 新的一场战斗，重置明灯愿计数
                MingxinEnterCount = 0;

                // 影画 1：获得 6 点青溟剑势，并将一张《照影》加入手卡
                if (CinimaCounter >= 1)
                {
                    Flash();
                    await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature, FirstTurnJianshi, Owner.Creature, null);

                    bool hasZhaoYing = PileType.Hand.GetPile(Owner).Cards.Any(c => c is ZhaoYing);

                    if (!hasZhaoYing)
                    {
                        var zhaoying = Owner.PlayerCombatState.AllCards.OfType<ZhaoYing>().FirstOrDefault();
                        if( zhaoying != null)
                            await CardPileCmd.Add(zhaoying, PileType.Hand);
                    }
                    //if (target == null) return;
                    //CardModel zhaoYing = combatState.CreateCard<ZhaoYing>(Owner);
                    //await CardPileCmd.AddGeneratedCardToCombat(zhaoYing, PileType.Hand, Owner);
                }

                // 影画 6：战斗开始时为自己施加 1 层明灯愿
                if (CinimaCounter >= 6)
                {
                    await PowerCmd.Apply<MingDengYuanPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, null);
                }
            }

            // 影画 3：每回合开始时获得 2 点力量
            if (CinimaCounter >= 3)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, StrengthPerTurn, Owner.Creature, null);
            }
        }

        /// <summary>
        /// 影画 2：明心镜状态下每消耗一次青溟剑势，随机升级等量卡牌并获得 1 点能量。
        /// </summary>
        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (CinimaCounter < 2) return;
            if (power is not QingmingJianshiPower) return;
            if (power.Owner != Owner?.Creature) return;

            // 只处理消耗（层数减少）；获得剑势时 amount 为正
            if (amount >= 0) return;

            // 战斗结束时清场造成的层数变化不算「消耗」
            if (!CombatManager.Instance.IsInProgress) return;
            if (Owner.Creature.CombatState == null) return;

            // 只在明心镜状态下生效
            if (!Owner.Creature.HasPower<MingxinJingPower>()) return;

            int consumed = (int)(-amount);
            if (consumed <= 0) return;

            UpgradeRandomCards(consumed, cardSource);
            await PlayerCmd.GainEnergy(1, Owner);
        }

        /// <summary>
        /// 影画 4/5/6：进入明心镜状态时的额外结算。
        /// </summary>
        public async Task OnMingxinJingEntered(PlayerChoiceContext choiceContext, Creature owner, int consumedPowerCount)
        {
            if (Owner?.Creature != owner) return;
            if (owner.CombatState == null) return;

            // 影画 4：每种被消耗的能力额外为所有敌人施加 10% 失衡易伤
            if (CinimaCounter >= 4 && consumedPowerCount > 0)
            {
                decimal vulnerable = consumedPowerCount * ExtraVulnerablePerPower;
                foreach (Creature enemy in owner.CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<DazeVulnPower>(choiceContext, enemy, vulnerable, owner, null);
                }
            }

            // 影画 5：进入明心镜状态时获得 8 点力量（在能力被消耗之后结算，因此会保留）
            if (CinimaCounter >= 5)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, owner, StrengthOnMingxinEnter, owner, null);
            }

            // 影画 6：之后每进入 2 次明心镜状态，为自己施加 1 层明灯愿
            if (CinimaCounter >= 6)
            {
                MingxinEnterCount++;
                if (MingxinEnterCount % 2 == 0)
                {
                    await PowerCmd.Apply<MingDengYuanPower>(choiceContext, owner, 1m, owner, null);
                }
            }
        }

        /// <summary>从战斗中的卡牌里随机升级指定数量的可升级卡牌。</summary>
        /// <param name="exclude">正在结算的卡牌本身（不参与随机）</param>
        private void UpgradeRandomCards(int count, CardModel? exclude = null)
        {
            Player? owner = Owner;
            if (owner?.PlayerCombatState == null) return;

            List<CardModel> candidates = owner.PlayerCombatState.AllCards
                .Where(c => c != null && c != exclude && c.IsUpgradable)
                .ToList();

            for (int i = 0; i < count && candidates.Count > 0; i++)
            {
                CardModel? card = owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
                if (card == null) break;

                candidates.Remove(card);
                CardCmd.Upgrade(card);
            }
        }
    }
}
