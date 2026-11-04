using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 策应 - 普通攻击卡
    /// 消耗1张手卡，对敌人造成消耗堆中卡牌数量的伤害。
    /// 升级：1费。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class CeYing : GuangCardBase, I2MingxinJing
    {
        public CeYing() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/ceYing.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(0, ValueProp.Move),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<ShiGan>(),
            HoverTipFactory.FromCard<YiWang>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
            //if (cardModel != null)
            //{
            //    await CardCmd.Exhaust(choiceContext, cardModel);
            //}
            await CardPileCmd.AddToCombatAndPreview<ShiGan>(Owner.Creature, PileType.Draw, 1, Owner);
            await CardPileCmd.AddToCombatAndPreview<YiWang>(Owner.Creature, PileType.Draw, 1, Owner);

            // 消耗堆中卡牌数量即为伤害值
            CardPile exhaustPile = PileType.Exhaust.GetPile(Owner);
            decimal damage = exhaustPile.Cards.Count();

            await DamageCmd.Attack(damage).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            //EnergyCost.UpgradeBy(-1); // 升级后1费
            RemoveKeyword(CardKeyword.Ethereal); // 升级后去掉虚无
        }
    }
}
