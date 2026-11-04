using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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
            new DamageVar(4, ValueProp.Move),
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
            CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
            if (cardModel != null)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
            }

            // 本回合已打出的攻击卡数量（本卡尚未结算完成，不计入）
            //int attacksThisTurn = CombatManager.Instance.History.CardPlaysFinished
            //    .Count((CardPlayFinishedEntry e)
            //        => e.CardPlay.Card.Owner == base.Owner
            //        && e.CardPlay.Card.Type == CardType.Attack
            //        && e.HappenedThisTurn(base.CombatState));

            //decimal damage = DynamicVars.Damage.BaseValue
            //    + attacksThisTurn * DynamicVars["DamagePerAttack"].IntValue;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .WithHitCount(2)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 失衡不在这里加：基类 GuangCardBase.AfterDamageGiven 已经会为本卡造成的伤害叠一次。
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars[DazeVarName].UpgradeValueBy(1);
        }
    }
}
