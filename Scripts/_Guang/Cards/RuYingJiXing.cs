using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
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
    /// 如影疾行 - 罕见攻击卡
    /// 对敌人造成 5 点伤害，失衡 2。
    /// 效果：如果敌人是攻击意图，获得 1 层滑溜。
    /// 升级：7 点伤害，失衡 3。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class RuYingJiXing : GuangCardBase
    {
        public RuYingJiXing() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("SlipperyGain", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 打出前先判断意图
            bool targetIsAttacking = cardPlay.Target?.Monster?.NextMove?.Intents
                ?.Any((AbstractIntent intent) => intent.IntentType == IntentType.Attack) == true;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡不在这里加：基类 GuangCardBase.AfterDamageGiven 已经会为本卡造成的伤害叠一次。

            // 效果：敌人是攻击意图时获得 1 层滑溜
            if (!targetIsAttacking) return;

            await PowerCmd.Apply<SlipperyPower>(choiceContext, Owner.Creature,
                DynamicVars["SlipperyGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2); // 升级后 7 点伤害
            DynamicVars[DazeVarName].UpgradeValueBy(1); // 升级后失衡 3
        }
    }
}
