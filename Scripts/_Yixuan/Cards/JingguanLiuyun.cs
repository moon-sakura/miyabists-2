using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 静观流云 - 3费Rare技能卡
    /// 发动2次将自身非闪能、支援点数的正面能力层数变为1.5倍，消耗
    /// 升级后变为2倍（能力是随机挑选，可重复）
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class JingguanLiuyun : YixuanCardBase
    {
        public JingguanLiuyun() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override bool HasEnergyCostX => true;

        protected override string ArtPath => "res://images/_YiXuan/cards/jingguanliuyun.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            //new DynamicVar("TriggerCount", 2),
            new DynamicVar("Multiplier", 150), // 1.5x, 百分比表示
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShannengPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var rng = Owner.RunState.Rng.CombatCardSelection;
            int triggerCount = ResolveEnergyXValue();
            double multiplier = DynamicVars["Multiplier"].IntValue / 100.0;

            // 收集符合条件的正面能力（排除闪能和支援点数）
            var buffs = Owner.Creature.Powers
                .Where(p => p.Type == PowerType.Buff
                    && p is not ShannengPower
                    && p is not SupportPointPower
                    && p.Amount > 0)
                .ToList();

            if (buffs.Count == 0) return;

            // 随机挑选 "发动2次"，可重复
            for (int i = 0; i < triggerCount; i++)
            {
                int pickIndex = rng.NextInt(0, buffs.Count);
                var picked = buffs[pickIndex];

                int newAmount = (int)Math.Ceiling(picked.Amount * (decimal)multiplier);
                picked.SetAmount(newAmount);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Multiplier"].UpgradeValueBy(50); // 150% -> 200%
        }
    }
}
