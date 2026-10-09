using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心镜变换工具（原卡 -> 明心镜卡）。
    /// 之后要加新的明心镜卡，只需要：
    ///   1. 让原卡实现 I2MingxinJing
    ///   2. 在下面的 Factories 里加一行
    /// </summary>
    public static class MingxinJingTransform
    {
        /// <summary>原卡类型 -> 对应的明心镜卡工厂</summary>
        private static readonly Dictionary<Type, Func<Player, CardModel>> Factories = new()
        {
            { typeof(FenShuiXing),  p => p.Creature.CombatState.CreateCard<MingxinFenShuiXing>(p) },
            { typeof(FuYaoShi),     p => p.Creature.CombatState.CreateCard<MingxinFuYaoShi>(p) },
            { typeof(ZhanLiuGuang), p => p.Creature.CombatState.CreateCard<MingxinZhanLiuGuang>(p) },
            { typeof(ZhiGe),        p => p.Creature.CombatState.CreateCard<MingxinBaoYi>(p) },
            { typeof(CeYing),       p => p.Creature.CombatState.CreateCard<MingxinCeYing>(p) },
            { typeof(FuYiQu),       p => p.Creature.CombatState.CreateCard<MingxinFuYiQu>(p) },
            { typeof(FeiGuang),     p => p.Creature.CombatState.CreateCard<MingxinFeiGuang>(p) },
            { typeof(CheJingLei),   p => p.Creature.CombatState.CreateCard<MingxinCheJingLei>(p) },
        };

        /// <summary>这张卡能否变成明心镜卡（实现了 I2MingxinJing 且登记过）</summary>
        public static bool CanTransform(CardModel card)
        {
            return card != null && card is I2MingxinJing && Factories.ContainsKey(card.GetType());
        }

        /// <summary>按类型判断能否变成明心镜卡</summary>
        public static bool CanTransform(Type originalType)
        {
            return originalType != null && Factories.ContainsKey(originalType);
        }

        /// <summary>
        /// 把一张原卡就地变换为对应的明心镜卡（原卡移出，明心镜卡加入手卡）。
        /// </summary>
        public static async Task<bool> Transform(PlayerChoiceContext choiceContext, CardModel original)
        {
            if (original == null) return false;
            if (!Factories.TryGetValue(original.GetType(), out Func<Player, CardModel> factory)) return false;

            Player owner = original.Owner;
            if (owner == null) return false;

            CardModel mingxinJingCard = factory(owner);
            if (mingxinJingCard == null) return false;

            // TODO: 保留原卡的升级状态（升级相关 API 待确认）
            // if (original.IsUpgraded) mingxinJingCard.Upgrade();

            // TODO: 原卡此处按"消耗"处理；若原卡不应进入消耗堆，改成对应的移除指令
            await CardCmd.Exhaust(choiceContext, original);

            // 明心镜形态加入手卡
            await CardPileCmd.AddGeneratedCardToCombat(mingxinJingCard, PileType.Hand, owner);
            return true;
        }

        /// <summary>
        /// 把手牌中所有可变换的原卡都变成明心镜卡（进入明心镜状态时调用）。
        /// 返回变换成功的张数。
        /// </summary>
        public static async Task<int> TransformAllInHand(PlayerChoiceContext choiceContext, Player owner)
        {
            CardPile handPile = PileType.Hand.GetPile(owner);
            List<CardModel> targets = handPile.Cards.Where(CanTransform).ToList();

            int count = 0;
            foreach (CardModel card in targets)
            {
                if (await Transform(choiceContext, card)) count++;
            }

            return count;
        }
    }
}
