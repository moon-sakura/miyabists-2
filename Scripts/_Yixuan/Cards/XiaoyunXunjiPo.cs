using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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
    [RegisterCard(typeof(YixuanCardPool))]
    internal class XiaoyunXunjiPo : YixuanAtkCardBase
    {
        public XiaoyunXunjiPo() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount", 3),
            new DynamicVar(ShannengVarName, 20),
            new DynamicVar(VigorVarName, 10),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Unblockable()
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            // 闪能20：活力
            await ShannengFunc(choiceContext, DynamicVars[ShannengVarName].IntValue, async () =>
            {
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars[VigorVarName].IntValue, Owner.Creature, this);
            });
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (cardSource != this || target == null || target.IsDead) return;

            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar))
            {
                await MiyabiCombatService.AddDaze(choiceContext, target, dazeVar, base.Owner.Creature);
            }

            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, 1m, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
