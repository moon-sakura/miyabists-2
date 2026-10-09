using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 引沧澜 - 初始攻击卡
    /// 对敌人造成5点伤害。将1张随机普通牌加入你的手牌。
    /// 升级：加入的普通牌添加保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class YinCangLan : GuangCardBase
    {
        public YinCangLan() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/yinCangLan.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new CardsVar(1),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 随机普通牌加入手牌
            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                CardModel card = CardFactory.GetDistinctForCombat(Owner, from c in Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                                                                       where c.Rarity == CardRarity.Common
                                                                       select c, 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (card == null) break;

                // 升级后：加入的随机普通牌附加保留
                if (IsUpgraded) card.AddKeyword(CardKeyword.Retain);

                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            // 升级效果（加入的普通牌附加保留）在 OnPlay 里根据 IsUpgraded 处理，此处无需改动数值
        }
    }
}
