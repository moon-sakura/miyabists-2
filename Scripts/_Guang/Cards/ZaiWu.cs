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
    /// 载物 - 罕见技能卡
    /// 明心镜状态下：消耗至多 3 点青溟剑势，恢复对应数量的能量。
    /// 非明心镜状态下：获得 1 点青溟剑势。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZaiWu : GuangCardBase
    {
        public ZaiWu() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MaxJianshiCost", 3),
            new DynamicVar("JianshiGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (Owner.Creature.HasPower<MingxinJingPower>())
            {
                // 明心镜状态：消耗至多 3 点青溟剑势，恢复对应数量的能量
                int available = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
                int spend = Math.Min(available, DynamicVars["MaxJianshiCost"].IntValue);
                if (spend <= 0) return;

                await CostJianshi(spend, choiceContext);
                await PlayerCmd.GainEnergy(spend, Owner);
                return;
            }

            // 非明心镜状态：获得青溟剑势
            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }
    }
}
