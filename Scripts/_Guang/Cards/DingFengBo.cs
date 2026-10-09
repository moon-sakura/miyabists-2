using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 定风波 - 罕见攻击卡
    /// 造成5点伤害，获得5点格挡，获得1点青溟剑势。
    /// 附加：被消耗时将1张随机非稀有牌加入你的手牌。
    /// 升级：7点伤害，7点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class DingFengBo : GuangCardBase
    {
        public DingFengBo() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/dingFengBo.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new BlockVar(5, ValueProp.Move),
            new DynamicVar("JianshiGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars.Block.UpgradeValueBy(2);
        }

        // 被消耗时将1张随机非稀有牌加入手牌
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            CardModel cardModel = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                                                                               where c.Rarity == CardRarity.Common || c.Rarity == CardRarity.Uncommon
                                                                               select c, 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (cardModel != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, base.Owner);
            }
        }
    }
}
