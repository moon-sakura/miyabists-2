using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心境·拂衣去（原卡：拂衣去）
    /// 获得3点格挡，下一张攻击卡可以免费打出。
    /// 被消耗时：在下三个回合获得1点能量。
    /// 升级：6点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinFuYiQu : GuangMingxinjingCardBase
    {
        public MingxinFuYiQu() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(3, ValueProp.Move),
            new EnergyVar(1),
            new DynamicVar("TurnCount", 3),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // 下一张攻击卡免费（FreeAttackPower）
            await PowerCmd.Apply<FreeAttackPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
        }

        // 被消耗时：在下三个回合获得1点能量
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            // ========== 待实现 ==========
            // TODO: 接下来 DynamicVars["TurnCount"] 个回合，每回合获得 DynamicVars.Energy 点能量。
            // 原版只有 EnergyNextTurnPower（只作用于下 1 个回合，Amount 表示能量数），
            // 无法表达"每回合 1 点、持续 3 回合"，需要一个自定义的持续型能力，待补充。
            await PowerCmd.Apply<RadiancePower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        }
    }
}
