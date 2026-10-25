using MegaCrit.Sts2.Core.CardSelection;
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
    /// 照影 - 初始技能卡
    /// 消耗1张手卡，获得1点青溟剑势。
    /// 附加：拥有6点青溟剑势则进入明心镜状态，回复所有能量，抽3张牌。
    /// 进入明心镜状态后本卡变为《明心镜·归尘》，并直接加入手卡。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhaoYing : GuangCardBase, I2MingxinJing
    {
        public ZhaoYing() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhaoYing.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiGain", 1),
            new DynamicVar("MingxinThreshold", 6),
            new DynamicVar("DrawCount", 3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 消耗1张手卡（本卡自身已在结算中，不会出现在手牌里）
            CardModel target = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext,
                player: Owner,
                filter: null,
                source: this)).FirstOrDefault();

            if (target != null)
            {
                await CardCmd.Exhaust(choiceContext, target);
            }

            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);

            // 附加：拥有6点青溟剑势则进入明心镜状态
            int jianshi = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
            if (jianshi >= DynamicVars["MingxinThreshold"].IntValue
                && !Owner.Creature.HasPower<MingxinJingPower>())
            {
                // 回复所有能量（把能量补满到本回合上限）
                int recoverEnergy = Owner.PlayerCombatState.MaxEnergy - Owner.PlayerCombatState.Energy;
                if(recoverEnergy > 0){
                    await PlayerCmd.GainEnergy(recoverEnergy, Owner);
                }
                //await PlayerCmd.SetEnergy(Owner.PlayerCombatState.MaxEnergy, Owner);

                // 抽牌（明心镜状态下抽到的卡会自动变为明心镜卡）
                await CardPileCmd.Draw(choiceContext, DynamicVars["DrawCount"].IntValue, Owner);

                // 进入明心镜状态。变牌、消耗正面能力、扣生命上限、失衡易伤都由能力自己处理
                await PowerCmd.Apply<MingxinJingPower>(choiceContext, Owner.Creature,
                    1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }
    }
}
