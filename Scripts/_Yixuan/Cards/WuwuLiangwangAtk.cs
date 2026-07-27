using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Patches;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 物我两忘·攻 - 0费Token攻击卡
    /// 造成消耗能力层数的玄墨伤害，消耗，保留
    /// </summary>
    [RegisterCard(typeof(StatusCardPool))]
    internal class WuwuLiangwangAtk : YixuanAtkCardBase
    {
        public WuwuLiangwangAtk() : base(0, CardRarity.Token, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_YiXuan/cards/wuwuLiangwangAtk.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(0, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar("ExhaustedPowerCount", 0),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Xuanmo,
            CardKeyword.Exhaust,
            CardKeyword.Retain,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Xuanmo),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 使用创建时设置的ExhaustedPowerCount作为伤害值
            int damageAmount = DynamicVars["ExhaustedPowerCount"].IntValue;

            await DamageCmd.Attack(damageAmount)
                .FromCard(this, cardPlay)
                .Unblockable()
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, DynamicVars["ExhaustedPowerCount"], base.Owner.Creature);
        }

        protected override void OnUpgrade()
        {
            // Token卡不升级
        }
    }
}
