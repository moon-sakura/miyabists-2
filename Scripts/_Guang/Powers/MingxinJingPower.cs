using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Guang.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 明心镜状态。
    /// 进入时：消耗所有正面能力（支援点数、青溟剑势除外），每消耗 1 种为所有敌人附加 10% 失衡易伤，
    ///         把战斗中所有 I2MingxinJing 的卡变为对应的明心镜卡，并直接加入一张《明心镜·归尘》到手中。
    /// 期间：抽到的原卡也会自动变为明心镜卡；《明心镜·归尘》不在手卡时每次出牌后自动补回手卡。
    /// 移除时：所有明心镜卡变回原卡（保留升级状态），《明心镜·归尘》自动消耗。
    /// </summary>
    internal class MingxinJingPower : ModPowerTemplate
    {
        /// <summary>每消耗 1 种正面能力，给所有敌人附加的失衡易伤（%）</summary>
        private const decimal VulnerablePerPower = 10m;

        /// <summary>进入状态时扣除的生命上限（%）</summary>
        private const decimal HpLossPercent = 10m;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/mingxinJing.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private static readonly string[] GuangStartVoices = { "guang_start_jiansuiwoyi", "guang_start_zhangjianzhuxie", "guang_start_zhanjinhongchen" };

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();

            Player owner = Owner.Player;
            if (owner == null) return;

            // 战斗立绘切换为明心镜形态
            MingxinJingVisuals.Enter(Owner);

            MiyabiAudioPlay.Random(GuangStartVoices);
            await MiyabiVideoService.PlayIfEnabledAsync("guang_change", mute: true);

            // 1. 消耗所有正面能力（支援点数与青溟剑势除外）
            List<PowerModel> consumed = Owner.Powers
                .Where(p => p != this
                    && p.Type == PowerType.Buff
                    && p is not SupportPointPower
                    && p is not QingmingJianshiPower)
                .ToList();

            foreach (PowerModel power in consumed)
            {
                await PowerCmd.Remove(power);
            }

            // 2. 每消耗 1 种正面能力，为所有敌人附加 10% 失衡易伤
            if (consumed.Count > 0)
            {
                decimal vulnerable = consumed.Count * VulnerablePerPower;
                foreach (Creature enemy in Owner.CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<DazeVulnPower>(choiceContext, enemy, vulnerable, Owner, null);
                }
            }

            // 2.5 影画/遗物的额外结算。
            // 光会消耗所有正面能力，所以相关效果不能做成能力，交给遗物自己结算。
            if (owner.Relics != null)
            {
                foreach (IMingxinJingEnterListener listener in owner.Relics.OfType<IMingxinJingEnterListener>())
                {
                    await listener.OnMingxinJingEntered(choiceContext, Owner, consumed.Count);
                }
            }

            // 3. 扣除 10% 生命上限（暂时关闭，测试后再决定是否加回来）
            // await CreatureCmd.LoseMaxHp(choiceContext, Owner, Owner.MaxHp * HpLossPercent / 100m, false);

            // 4. 所有原卡进入明心镜形态
            await MingxinJingTransform.TransformAllInCombat(owner);

            // 5. 直接加入一张《明心镜·归尘》（手卡没空位时先放到抽卡堆顶，之后每次出牌再补进手卡）
            await AddGuiChenIfNeeded();
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"退出明心镜状态"，不处理变回原卡
            if (!CombatManager.Instance.IsInProgress) return;

            // 退出明心镜状态：所有明心镜卡变回原卡（保留升级状态）
            if (oldOwner?.Player == null) return;

            // 战斗立绘换回当前选择的战斗皮肤
            MingxinJingVisuals.Exit(oldOwner);

            Player owner = oldOwner.Player;
            await MingxinJingTransform.RevertAllInCombat(owner);

            // 《明心镜·归尘》自动消耗（先快照再消耗，避免遍历时牌堆变动）。
            // 正在被打出的那张归尘此时还在 Play 堆里，由它自己出牌结束时消耗。
            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();
            List<CardModel> guiChenCards = new[] { PileType.Hand, PileType.Draw, PileType.Discard }
                .SelectMany(pileType => pileType.GetPile(owner).Cards)
                .Where(c => c is MingxinGuiChen)
                .ToList();

            foreach (CardModel guiChen in guiChenCards)
            {
                await CardCmd.Exhaust(choiceContext, guiChen);
            }
        }

        public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
        /// <summary>明心镜状态下抽到的原卡也立刻变为明心镜卡</summary>
        {
            if (card == null || card.Owner != Owner.Player) return;
            if (!MingxinJingTransform.CanTransform(card)) return;
            await MingxinJingTransform.Transform(card);
        }

        /// <summary>每次出牌后，如果《明心镜·归尘》不在手卡就补回手卡</summary>
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay?.Card?.Owner != Owner?.Player) return;
            await AddGuiChenIfNeeded();
        }

        /// <summary>
        /// 确保《明心镜·归尘》在手卡里：
        /// 已经在手卡就不动；战斗牌堆里已经有一张就把它挪进手卡；
        /// 一张都没有就新生成一张。
        /// 手卡没位置时放到抽卡堆顶部，之后每次出牌再尝试补进手卡。
        /// </summary>
        public async Task AddGuiChenIfNeeded()
        {
            Player? owner = Owner?.Player;
            if (owner?.PlayerCombatState == null) return;

            CardPile hand = PileType.Hand.GetPile(owner);
            if (hand.Cards.Any(c => c is MingxinGuiChen)) return;

            // 先快照再处理，避免挪动卡牌时改动牌堆导致遍历失效
            List<CardModel> cardsInCombatPiles = owner.PlayerCombatState.AllPiles
                .Where(p => p.Type != PileType.Play)
                .SelectMany(p => p.Cards)
                .ToList();

            CardModel? existing = cardsInCombatPiles.FirstOrDefault(c => c is MingxinGuiChen);
            if (existing != null)
            {
                await MingxinJingTransform.PlaceGuiChenInHand(owner, existing);
                return;
            }

            // 战斗里还没有归尘：直接生成一张
            CardModel? guiChen = owner.Creature.CombatState?.CreateCard<MingxinGuiChen>(owner);
            if (guiChen == null) return;

            if (hand.Cards.Count < CardPile.MaxCardsInHand)
            {
                await CardPileCmd.AddGeneratedCardToCombat(guiChen, PileType.Hand, owner);
            }
            else
            {
                await CardPileCmd.AddGeneratedCardToCombat(guiChen, PileType.Draw, owner, CardPilePosition.Top);
            }
        }
    }
}
