using MegaCrit.Sts2.Core.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心镜状态卡牌的父类。
    /// 所有明心镜卡牌继承它即可自动带上 IMingxinJing 标记，
    /// 用 `card is IMingxinJing` 就能判断一张卡是否处于明心镜状态。
    /// </summary>
    public abstract class GuangMingxinjingCardBase : GuangCardBase, IMingxinJing
    {
        protected GuangMingxinjingCardBase(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        // 明心镜卡牌暂时统一使用这张卡图
        protected override string ArtPath => "res://images/_Guang/cards/mingxinjing_common.png";
    }
}
