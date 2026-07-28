using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using Miyabists2.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(CurseCardPool))]
    internal class HollowErosion : MiyabiCardBase
    {
        // 卡图路径（暂用通用路径，后续可替换）
        protected override string ArtPath => "res://images/cards/hollow.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Unplayable,
            CardKeyword.Eternal,
        ];

        public HollowErosion()
            : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
        {
        }

        public override int MaxUpgradeLevel => 0;

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if(card == this)
            {
                HollowExingyilai optionEx = base.Owner.Creature.CombatState?.CreateCard<HollowExingyilai>(base.Owner);
                HollowGuodujinshe optionGd = base.Owner.Creature.CombatState?.CreateCard<HollowGuodujinshe>(base.Owner);
                HollowJishengyijun optionJs = base.Owner.Creature.CombatState?.CreateCard<HollowJishengyijun>(base.Owner);
                HollowJuwukongju optionJw = base.Owner.Creature.CombatState?.CreateCard<HollowJuwukongju>(base.Owner);
                HollowTanlanwudu optionTanlanwudu = base.Owner.Creature.CombatState?.CreateCard<HollowTanlanwudu>(base.Owner);
                HollowZhijuetonghua optionTh = base.Owner.Creature.CombatState?.CreateCard<HollowZhijuetonghua>(base.Owner);

                if (optionEx != null && optionGd != null && optionJs != null && optionJw != null && optionTanlanwudu != null && optionTh != null)
                {
                    List<CardModel> options = new List<CardModel> { optionEx, optionGd, optionJs, optionJw, optionTanlanwudu, optionTh };

                    for (int i = 0; i < 3; i++)
                    {
                        int result = base.Owner.RunState.Rng.Shuffle.NextInt(1, options.Count + 1);
                        options.RemoveRange(result - 1, 1);
                    }

                    CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
                    if (chosen is HollowExingyilai)
                    {
                        await PowerCmd.Apply<HollowExingyilaiPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    }
                    else if (chosen is HollowGuodujinshe)
                    {
                        await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
                    }
                    else if (chosen is HollowJishengyijun)
                    {
                        await PowerCmd.Apply<HollowJishengyijunPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    }
                    else if (chosen is HollowJuwukongju)
                    {
                        await PowerCmd.Apply<HollowJuwukongjuPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    }
                    else if (chosen is HollowTanlanwudu)
                    {
                        await PowerCmd.Apply<HollowTanlanwuduPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    }
                    else if (chosen is HollowZhijuetonghua)
                    {
                        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                        await PowerCmd.Apply<HollowZhijuetonghuaPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
                    }
                }
            }
        }

        // ========== 具体效果由你来实现 ==========
        // 示例：可以在这里覆写 OnTurnStart、OnCardPlayed 等钩子方法
        // 参考 MiyabiCardBase 以及其他卡牌的实现方式
    }
}
