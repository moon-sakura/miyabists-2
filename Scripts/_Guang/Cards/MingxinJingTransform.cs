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
    /// 明心镜变换工具（原卡 &lt;-&gt; 明心镜卡，双向就地变换）。
    /// 之后要加新的明心镜卡，只需要：
    ///   1. 让原卡实现 I2MingxinJing
    ///   2. 在下面的静态构造里加一行 Register&lt;原卡, 明心镜卡&gt;()
    /// </summary>
    public static class MingxinJingTransform
    {
        /// <summary>参与明心镜变换的战斗内牌堆（抽牌堆 / 手卡 / 弃牌堆 / 消耗堆）</summary>
        private static readonly PileType[] CombatPiles =
        [
            PileType.Draw,
            PileType.Hand,
            PileType.Discard,
            PileType.Exhaust,
            PileType.Play,
        ];

        /// <summary>原卡类型 -> 明心镜卡工厂</summary>
        private static readonly Dictionary<Type, Func<Player, CardModel>> Factories = new();

        /// <summary>明心镜卡类型 -> 原卡工厂（退出明心镜状态时用）</summary>
        private static readonly Dictionary<Type, Func<Player, CardModel>> ReverseFactories = new();

        static MingxinJingTransform()
        {
            Register<FenShuiXing, MingxinFenShuiXing>();
            Register<FuYaoShi, MingxinFuYaoShi>();
            Register<ZhanLiuGuang, MingxinZhanLiuGuang>();
            Register<BaoYi, MingxinBaoYi>();
            Register<ZhaoYing, MingxinGuiChen>();
            Register<CeYing, MingxinCeYing>();
            Register<FuYiQu, MingxinFuYiQu>();
            Register<FeiGuang, MingxinFeiGuang>();
            Register<CheJingLei, MingxinCheJingLei>();
        }

        /// <summary>登记一对「原卡 -> 明心镜卡」，正反两个方向同时登记，不会写漏</summary>
        private static void Register<TOriginal, TMingxin>()
            where TOriginal : CardModel
            where TMingxin : CardModel
        {
            Factories[typeof(TOriginal)] = p => p.Creature.CombatState.CreateCard<TMingxin>(p);
            ReverseFactories[typeof(TMingxin)] = p => p.Creature.CombatState.CreateCard<TOriginal>(p);
        }

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

        /// <summary>这张卡是否是能变回原卡的明心镜卡</summary>
        public static bool CanRevert(CardModel card)
        {
            return card != null && card is IMingxinJing && ReverseFactories.ContainsKey(card.GetType());
        }

        /// <summary>把一张原卡就地变为对应的明心镜卡（不改变所在牌堆，保留升级状态）。失败返回 null</summary>
        public static async Task<CardModel?> Transform(CardModel original)
        {
            return await Swap(original, Factories);
        }

        /// <summary>把一张明心镜卡就地变回原卡（不改变所在牌堆，保留升级状态）。失败返回 null</summary>
        public static async Task<CardModel?> Revert(CardModel mingxinJingCard)
        {
            return await Swap(mingxinJingCard, ReverseFactories);
        }

        /// <summary>把战斗中所有可变换的原卡都变成明心镜卡，返回变换成功的张数</summary>
        public static async Task<int> TransformAllInCombat(Player owner)
        {
            return await SwapAll(owner, CanTransform, Transform);
        }

        /// <summary>把战斗中所有明心镜卡都变回原卡，返回变换成功的张数</summary>
        public static async Task<int> RevertAllInCombat(Player owner)
        {
            return await SwapAll(owner, CanRevert, Revert);
        }

        private static async Task<CardModel?> Swap(CardModel original, Dictionary<Type, Func<Player, CardModel>> table)
        {
            if (original == null) return null;
            if (!table.TryGetValue(original.GetType(), out Func<Player, CardModel> factory)) return null;

            Player owner = original.Owner;
            if (owner == null) return null;

            CardModel replacement = factory(owner);
            if (replacement == null) return null;

            // 保留原卡的升级状态
            for (int i = 0; i < original.CurrentUpgradeLevel; i++)
            {
                replacement.UpgradeInternal();
            }

            // 就地替换：卡牌留在原来的牌堆里
            await CardCmd.Transform(original, replacement);

            // 《明心镜·归尘》由照影变化而来，变化完成后直接加入手卡
            if (replacement is MingxinGuiChen)
            {
                await PlaceGuiChenInHand(owner, replacement);
            }

            return replacement;
        }

        /// <summary>
        /// 把《明心镜·归尘》放进手卡：手卡有空位就加入手卡，手卡满了则放到抽卡堆顶部。
        /// </summary>
        public static async Task PlaceGuiChenInHand(Player owner, CardModel guiChen)
        {
            if (owner == null || guiChen == null) return;

            CardPile hand = PileType.Hand.GetPile(owner);
            if (guiChen.Pile == hand) return;

            if (hand.Cards.Count < CardPile.MaxCardsInHand)
            {
                await CardPileCmd.Add(guiChen, hand);
                return;
            }

            CardPile draw = PileType.Draw.GetPile(owner);
            if (guiChen.Pile != draw)
            {
                await CardPileCmd.Add(guiChen, draw, CardPilePosition.Top);
            }
        }

        private static async Task<int> SwapAll(Player owner, Func<CardModel, bool> filter, Func<CardModel, Task<CardModel?>> swap)
        {
            if (owner == null) return 0;

            int count = 0;
            foreach (PileType pileType in CombatPiles)
            {
                // 先快照再变换，避免变换过程中改动牌堆导致遍历失效
                List<CardModel> targets = pileType.GetPile(owner).Cards
                    .Where(c => c != null && filter(c))
                    .ToList();

                foreach (CardModel card in targets)
                {
                    if (await swap(card) != null) count++;
                }
            }

            return count;
        }
    }
}
