using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhaoYing : GuangCardBase
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
            // TODO: 明心镜能力写好后取消注释
            // HoverTipFactory.FromPower<MingxinJingPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // TODO: 消耗1张手卡（需要手牌选择界面）
            // await CardSelectCmd.FromChooseACardScreen(...) -> CardCmd.Exhaust(...)

            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);

            // TODO: 明心镜判定
            // 若 Owner.Creature.GetPowerAmount<QingmingJianshiPower>() >= MingxinThreshold：
            //   进入明心镜状态（具体实现待明心镜能力完成后接入）
            //   回复所有能量
            //   抽 DynamicVars["DrawCount"] 张牌
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }
    }
}
