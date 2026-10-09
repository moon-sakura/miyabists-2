using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心镜·斩流光·极（token）
    /// 对所有敌人造成2次8点伤害，失衡6，消耗。
    /// 附加：消耗2点青溟剑势，再打出1次。
    /// 升级：10点伤害，失衡8。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinZhanLiuGuangJi : GuangMingxinjingCardBase
    {
        public MingxinZhanLiuGuangJi() : base(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8, ValueProp.Move),
            new DynamicVar(DazeVarName, 6),
            new DynamicVar("HitCount", 2),
            new DynamicVar(JianshiVarName, 2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 附加：消耗2点青溟剑势 -> 再打出1次
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                    .WithHitCount(DynamicVars["HitCount"].IntValue)
                    .TargetingAllOpponents(Owner.Creature.CombatState)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars[DazeVarName].UpgradeValueBy(2);
        }
    }
}
