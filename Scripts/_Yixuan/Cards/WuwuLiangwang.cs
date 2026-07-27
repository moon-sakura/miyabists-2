using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts._Yixuan.Powers.CinimaPower;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 物我两忘 - X费Uncommon技能卡
    /// 消除所有能力，舍弃所有手牌，然后随机添加舍弃手牌数的特殊卡牌（卡牌数值由消除能力数决定），消耗
    /// 升级后去掉消耗
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class WuwuLiangwang : YixuanCardBase
    {
        public WuwuLiangwang() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override bool HasEnergyCostX => true;

        //protected override string ArtPath => "res://images/_YiXuan/cards/wuwuLiangwang.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            // 卡牌数值由消除能力数动态决定，此处仅声明占位
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<WuwuLiangwangAtk>(),
            HoverTipFactory.FromCard<WuwuLiangwangDef>(),
            HoverTipFactory.FromCard<WuwuLiangwangSus>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int xValue = ResolveEnergyXValue();
            var rng = Owner.RunState.Rng.Shuffle;

            // 1. 消除所有能力（移除自身所有Power）
            var powersToRemove = Owner.Creature.Powers.ToList();
            int exhaustedPowerAmount = 0;

            foreach (var power in powersToRemove)
            {
                if(power is not ShannengPower && power is not SupportPointPower)
                {
                    if(power.Type != PowerType.None)
                    {
                        GD.Print($"[MIYABIMOD] WUWOLIANGWANG REMOVE {power.Title.ToString()}");
                        exhaustedPowerAmount += power.Amount;
                        await PowerCmd.Remove(power);
                    }
                }
            }

            // 2. 舍弃所有手牌（排除自身，自身在打出时已不在手牌中）
            var handCards = Owner.PlayerCombatState.Hand.Cards.ToList();
            int discardCount = handCards.Count;

            foreach (var card in handCards)
            {
                await CardCmd.Discard(choiceContext, card);
            }

            // 3. 随机添加特殊卡牌（数量=舍弃手牌数，数值由消除能力数决定）
            for (int i = 0; i < discardCount; i++)
            {
                CardModel specialCard = null;
                if (exhaustedPowerAmount >= 10)
                {
                    int tokenType = rng.NextInt(0, 3);
                    specialCard = tokenType switch
                    {
                        0 => base.Owner.Creature.CombatState.CreateCard<WuwuLiangwangAtk>(base.Owner.Creature.Player),
                        1 => base.Owner.Creature.CombatState.CreateCard<WuwuLiangwangDef>(base.Owner.Creature.Player),
                        _ => base.Owner.Creature.CombatState.CreateCard<WuwuLiangwangSus>(base.Owner.Creature.Player),
                    };
                }
                else
                {
                    int tokenType = rng.NextInt(0, 2);
                    specialCard = tokenType switch
                    {
                        0 => base.Owner.Creature.CombatState.CreateCard<WuwuLiangwangDef>(base.Owner.Creature.Player),
                        _ => base.Owner.Creature.CombatState.CreateCard<WuwuLiangwangAtk>(base.Owner.Creature.Player),
                    };
                }


                // 将消除能力数写入卡牌，由各Token卡在OnPlay中读取使用
                if (specialCard is not WuwuLiangwangSus)
                    specialCard.DynamicVars["ExhaustedPowerCount"].BaseValue = exhaustedPowerAmount;
                else
                {
                    specialCard.DynamicVars["ExhaustedPowerCount"].BaseValue = exhaustedPowerAmount / 10;
                    specialCard.DynamicVars.Energy.BaseValue = exhaustedPowerAmount / 10;
                }

                await CardPileCmd.AddGeneratedCardToCombat(specialCard, PileType.Hand, Owner, CardPilePosition.Random);
            }
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
