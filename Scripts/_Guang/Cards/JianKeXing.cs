using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    /// 剑客行 - 稀有攻击卡
    /// 对敌人造成 6 点伤害，本回合每使用过一张卡，造成伤害 +1。
    /// 效果：造成等量失衡。
    /// 升级：8 点伤害。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class JianKeXing : GuangCardBase
    {
        public JianKeXing() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar("DamagePerCard", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 本回合已经打出过的卡牌数量（当前这张卡此时还没结算完，不会被算进去）
            int playedThisTurn = CombatManager.Instance.History.CardPlaysFinished.Count(
                (CardPlayFinishedEntry e)
                    => e.CardPlay.Card.Owner == Owner
                    && e.HappenedThisTurn(CombatState));

            decimal damage = DynamicVars.Damage.BaseValue
                + playedThisTurn * DynamicVars["DamagePerCard"].IntValue;

            await DamageCmd.Attack(damage).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡在 AfterDamageGiven 里按实际伤害叠加
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2); // 升级后 8 点伤害
        }

        /// <summary>
        /// 效果：造成等量失衡（按这次实际造成的伤害计算）。
        /// 不走基类的 DazeVar 逻辑，因为这里没有固定的失衡值。
        /// </summary>
        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (cardSource != this || target == null || target.IsDead) return;
            if (result == null || result.TotalDamage <= 0) return;

            await MiyabiCombatService.AddDaze(choiceContext, target,
                new DynamicVar(DazeVarName, result.TotalDamage), Owner.Creature);
        }
    }
}
