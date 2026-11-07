using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    /// 明心镜·归尘 - 进入明心镜状态时自动加入手卡，退出明心镜状态后被消耗
    /// 对所有敌人造成3次3点伤害，失衡2，退出明心镜状态。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinGuiChen : GuangMingxinjingCardBase
    {
        public MingxinGuiChen() : base(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount", 3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 退出明心镜状态（能力被移除时会把所有明心镜卡变回原卡，并消耗其他归尘）
            if (Owner.Creature.HasPower<MingxinJingPower>())
            {
                await PowerCmd.Remove(Owner.Creature.GetPower<MingxinJingPower>());
            }

            // 本卡此时还在 Play 堆里，退出状态时扫不到它，由自己消耗
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}
