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
    /// 明心镜·斩流光（原卡：斩流光）
    /// 对所有敌人造成2次6点伤害，失衡2。
    /// 附加：消耗1点青溟剑势，选择一张《灭》或者《极》加入手卡。
    /// 升级：8点伤害，失衡3。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinZhanLiuGuang : GuangMingxinjingCardBase
    {
        public MingxinZhanLiuGuang() : base(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount", 2),
            new DynamicVar(JianshiVarName, 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromCard<MingxinZhanLiuGuangMie>(),
            HoverTipFactory.FromCard<MingxinZhanLiuGuangJi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 附加：消耗1点青溟剑势 -> 选择一张《灭》或者《极》加入手卡
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                List<CardModel> options =
                [
                    Owner.Creature.CombatState.CreateCard<MingxinZhanLiuGuangMie>(Owner),
                    Owner.Creature.CombatState.CreateCard<MingxinZhanLiuGuangJi>(Owner),
                ];

                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
                if (chosen != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
                }
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars[DazeVarName].UpgradeValueBy(1);
        }
    }
}
