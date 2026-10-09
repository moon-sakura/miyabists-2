using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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
    /// 明心镜·掣惊雷（原卡：掣惊雷）
    /// 对所有敌人造成4次2点伤害，失衡4。
    /// 附加：消耗1点青溟剑势，消耗这张卡。被消耗时获得1层超巨化。
    /// 升级：8点伤害。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinCheJingLei : GuangMingxinjingCardBase
    {
        public MingxinCheJingLei() : base(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(2, ValueProp.Move),
            new DynamicVar(DazeVarName, 4),
            new DynamicVar("HitCount", 4),
            new DynamicVar(JianshiVarName, 1),
            new DynamicVar("Giant", 1),
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
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 附加：消耗1点青溟剑势 -> 消耗这张卡
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                await CardCmd.Exhaust(choiceContext, this);
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(6);
        }

        // 被消耗时获得超巨化
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            // 超巨化（GigantificationPower）：下一张攻击牌造成三倍伤害
            await PowerCmd.Apply<GigantificationPower>(choiceContext, Owner.Creature,
                DynamicVars["Giant"].IntValue, Owner.Creature, this);
        }
    }
}
