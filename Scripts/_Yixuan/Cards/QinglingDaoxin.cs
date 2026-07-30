using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 清灵道心 - 2费Common技能卡
    /// 失去最大生命值15%的生命值，消除自身所有负面效果
    /// 升级后变为1费
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QinglingDaoxin : YixuanCardBase
    {
        public QinglingDaoxin() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/qinglingDaoxin.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("HpLossPercent", 15),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 失去最大生命值15%的生命值
            decimal hpLoss = Owner.Creature.MaxHp * DynamicVars["HpLossPercent"].IntValue / 100m;
            if (hpLoss > 0)
            {
                await CreatureCmd.Damage(choiceContext, Owner.Creature, hpLoss,
                    ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature);
            }

            // 消除自身所有负面效果（Debuff类型的Power）
            var debuffs = Owner.Creature.Powers
                .Where(p => p.Type == PowerType.Debuff 
                || (p is StrengthPower && p.Amount < 0)
                || (p is DexterityPower && p.Amount < 0))
                .ToList();

            foreach (var debuff in debuffs)
            {
                await PowerCmd.Remove(debuff);
            }

            await CreatureCmd.GainBlock(Owner.Creature, hpLoss, ValueProp.Unpowered, cardPlay);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
