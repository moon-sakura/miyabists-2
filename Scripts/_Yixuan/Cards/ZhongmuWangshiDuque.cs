using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
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
    internal class ZhongmuWangshiDuque : YixuanPartnerCardBase
    {
        public ZhongmuWangshiDuque() : base(1, CardRarity.Uncommon, TargetType.RandomEnemy, CardType.Attack)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/zhongmuWangshiDuque.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar("HitCount", 3),
            new DynamicVar("LifeLoss", 6),
            new BlockVar(4,ValueProp.Unpowered),
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
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 失去6点生命值
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars["LifeLoss"].IntValue,
                ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature);

            await base.OnPlay(choiceContext, cardPlay);

            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .Unblockable()
                    .TargetingRandomOpponents(Owner.Creature.CombatState)
                    .WithHitCount(DynamicVars["HitCount"].IntValue)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(choiceContext);
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if(dealer == Owner.Creature && cardSource == this && target.IsMonster && target.Monster.IntendsToAttack)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
