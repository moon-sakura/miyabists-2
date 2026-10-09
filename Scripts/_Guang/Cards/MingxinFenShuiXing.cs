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
    /// 明心镜·分水行（原卡：分水行）
    /// 造成12点伤害，失衡2。
    /// 附加：消耗1点青溟剑势，消耗1张《明心镜·扶摇势》。
    /// 升级：15点伤害，失衡3。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinFenShuiXing : GuangMingxinjingCardBase
    {
        public MingxinFenShuiXing() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(12, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar(JianshiVarName, 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromCard<MingxinFuYaoShi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡由 GuangCardBase.AfterDamageGiven 自动结算

            // 附加：消耗1点青溟剑势 -> 消耗1张《明心镜·扶摇势》
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                CardModel target = PileType.Hand.GetPile(Owner).Cards.FirstOrDefault(c => c is MingxinFuYaoShi);
                if (target != null)
                {
                    await CardCmd.Exhaust(choiceContext, target);
                }
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            DynamicVars[DazeVarName].UpgradeValueBy(1);
        }
    }
}
