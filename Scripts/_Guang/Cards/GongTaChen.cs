using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 共踏尘 - 稀有技能卡（X 费）
    /// 消耗所有手卡，将对应数量的《遗忘》加入手卡；
    /// 然后如果青溟剑势大于 6 点，恢复所有能量，进入明心镜状态。
    /// 被消耗时获得 1 层黑暗之拥。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class GongTaChen : GuangCardBase
    {
        public GongTaChen() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        /// <summary>X 费卡：显示与结算都使用 X</summary>
        protected override bool HasEnergyCostX => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiThreshold", 6),
            new DynamicVar("DarkEmbraceGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
            HoverTipFactory.FromPower<DarkEmbracePower>(),
            HoverTipFactory.FromCard<YiWang>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int x = ResolveEnergyXValue();

            // 先消耗所有手卡
            // TODO: 原描述被截断（“…手卡，将对应数量的遗忘加入手卡”）。
            //       这里按“消耗所有手卡，加入 X 张遗忘”实现，请确认“对应数量”指的是 X 还是消耗张数。
            List<CardModel> handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
            foreach (CardModel card in handCards)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }

            // 加入 X 张遗忘
            for (int i = 0; i < x; i++)
            {
                CardModel? forget = Owner.Creature.CombatState?.CreateCard<YiWang>(Owner);
                if (forget != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(forget, PileType.Hand, Owner);
                }
            }

            // 青溟剑势大于阈值：恢复所有能量，进入明心镜状态
            int jianshi = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
            if (jianshi > DynamicVars["JianshiThreshold"].IntValue)
            {
                int recoverEnergy = Owner.PlayerCombatState.MaxEnergy - Owner.PlayerCombatState.Energy;
                if (recoverEnergy > 0)
                {
                    await PlayerCmd.GainEnergy(recoverEnergy, Owner);
                }
                //await PlayerCmd.SetEnergy(Owner.PlayerCombatState.MaxEnergy, Owner);
                await PowerCmd.Apply<MingxinJingPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }

        // 被消耗时获得 1 层黑暗之拥
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<DarkEmbracePower>(choiceContext, Owner.Creature,
                DynamicVars["DarkEmbraceGain"].IntValue, Owner.Creature, this);
        }
    }
}
