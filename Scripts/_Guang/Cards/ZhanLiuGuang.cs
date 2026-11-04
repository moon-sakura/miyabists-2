using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 斩流光 - 普通攻击卡
    /// 对所有敌人造成4点伤害，失衡1。附加：被消耗时获得1点青溟剑势。
    /// 升级：6点伤害，失衡2。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhanLiuGuang : GuangCardBase, I2MingxinJing
    {
        public ZhanLiuGuang() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhanLiuGuang.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(9, ValueProp.Move),
            new DynamicVar(DazeVarName, 4),
            new DynamicVar("JianshiOnExhaust", 2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromCard<ShiGan>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CardPileCmd.AddToCombatAndPreview<ShiGan>(Owner.Creature, PileType.Hand, 1, Owner);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡不在这里加：基类 GuangCardBase.AfterDamageGiven 会按每个被命中的敌人各叠一次。
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars[DazeVarName].UpgradeValueBy(3);
        }

        // 被消耗时获得青溟剑势
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiOnExhaust"].IntValue, Owner.Creature, this);
        }
    }
}
