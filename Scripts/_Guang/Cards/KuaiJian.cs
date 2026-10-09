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
    /// 快剑 - 初始攻击卡
    /// 造成6点伤害，失衡1。被消耗时获得1点力量。
    /// 升级：9点伤害，失衡2。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class KuaiJian : GuangCardBase
    {
        public KuaiJian() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/kuaiJian.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 1),
            new DynamicVar("StrengthOnExhaust", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<StrengthPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
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

        // 被消耗时获得力量
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature,
                DynamicVars["StrengthOnExhaust"].IntValue, Owner.Creature, this);
        }
    }
}
