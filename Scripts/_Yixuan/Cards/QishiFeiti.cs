using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Patches;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QishiFeiti : YixuanPartnerCardBase
    {
        public QishiFeiti() : base(2, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/qishiFeiti.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar("HitCount", 3),
            new DynamicVar("LifeLoss", 5),
            new BlockVar(12,ValueProp.Unpowered),
            new DynamicVar(SupportVarName, 1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            MiyabiKeywords.Mingpo,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 失去5点生命值
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars["LifeLoss"].IntValue,
                ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Unblockable()
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            // 生命低于25%时获得12点格挡
            if ((decimal)Owner.Creature.CurrentHp / Owner.Creature.MaxHp < 0.25m)
            {
                await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
                {
                    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
                });
            }
        }

        

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(cardSource == this)
            {
                // 根据已损失生命值百分比计算伤害倍率
                decimal missingPercent = 1m - ((decimal)Owner.Creature.CurrentHp / Owner.Creature.MaxHp);
                decimal damageMultiplier = 1m + missingPercent;

                return damageMultiplier;
            }
            return 1m;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
