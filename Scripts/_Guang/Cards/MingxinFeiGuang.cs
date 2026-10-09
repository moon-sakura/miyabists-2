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
    /// 明心境·飞光（原卡：飞光）
    /// 对所有敌人造成5点伤害，失衡3。
    /// 附加：消耗1点青溟剑势，将一张自身的复制加入手卡。
    /// 升级：7点伤害。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinFeiGuang : GuangMingxinjingCardBase
    {
        public MingxinFeiGuang() : base(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar(JianshiVarName, 1),
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
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 附加：消耗1点青溟剑势 -> 将一张自身的复制加入手卡
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                CardModel copy = this.CreateClone();
                if (copy != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
                }
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
        }
    }
}
