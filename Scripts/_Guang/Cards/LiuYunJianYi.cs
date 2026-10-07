using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 流云剑意 - 普通攻击卡
    /// 造成6点伤害，失衡2。附加：本回合每使用1张攻击卡，伤害+1。
    /// 升级：9点伤害，失衡3。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class LiuYunJianYi : GuangCardBase
    {
        public LiuYunJianYi() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/liuYunJianYi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("DamagePerAttack", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // TODO: 附加效果 - 本回合每使用1张攻击卡，伤害+1
            // 需要在计算伤害前叠加 DynamicVars["DamagePerAttack"] 的值。
            decimal damage = DynamicVars.Damage.BaseValue;

            await DamageCmd.Attack(damage).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, DynamicVars[DazeVarName], base.Owner.Creature);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            DynamicVars[DazeVarName].UpgradeValueBy(1);
        }
    }
}
