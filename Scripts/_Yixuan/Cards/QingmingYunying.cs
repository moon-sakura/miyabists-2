using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{   
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QingmingYunying:YixuanAtkCardBase
    {
        public QingmingYunying() : base(1, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/qingmingYunying.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Xuanmo,
            MiyabiKeywords.EndSkill,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount",8),
            new DynamicVar("Heal",25),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShufaZhi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["HitCount"].IntValue).FromCard(this)
            .Unblockable()
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, 25, Owner.Creature, this);
            }

            await CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp * DynamicVars["Heal"].BaseValue / 100);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
