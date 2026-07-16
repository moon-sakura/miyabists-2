using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 术道归一 - 1费Uncommon技能卡
    /// 失去所有格挡，对敌人造成失去格挡数量的玄墨伤害，消耗2
    /// 升级后变为0费
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ShudaoGuiyi : YixuanCardBase
    {
        public ShudaoGuiyi() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/shudaoGuiyi.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Xuanmo,
            MiyabiKeywords.ExhaustX,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(ExhaustCountVarName, GetExhaustUses()),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Xuanmo),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            int lostBlock = (int)Owner.Creature.Block;

            // 造成失去格挡数量的玄墨（不可抵挡）伤害
            if (lostBlock > 0)
            {
                await DamageCmd.Attack(lostBlock)
                    .FromCard(this, cardPlay)
                    .Unblockable()
                    .Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(choiceContext);

                await CreatureCmd.GainBlock(Owner.Creature, -lostBlock, ValueProp.Unpowered, null);
            }

            // 失去所有格挡
            //if (lostBlock > 0)
            //{
                
            //}

            // 尝试消耗（消耗2）
            await TryExhaustAfterUse(choiceContext, cardPlay);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
