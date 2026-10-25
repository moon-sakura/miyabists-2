using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 燕袭 - 罕见攻击卡
    /// 对敌人造成 4 点伤害，失衡 2。
    /// 效果：获得 1 层独白。
    /// 升级：6 点伤害，失衡 3。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class YanXi : GuangCardBase
    {
        public YanXi() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("MonologueGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<MonologuePower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡不在这里加：基类 GuangCardBase.AfterDamageGiven 已经会为本卡造成的伤害叠一次。

            // 效果：获得 1 层独白
            await PowerCmd.Apply<MonologuePower>(choiceContext, Owner.Creature,
                DynamicVars["MonologueGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2); // 升级后 6 点伤害
            DynamicVars[DazeVarName].UpgradeValueBy(1); // 升级后失衡 3
        }
    }
}
