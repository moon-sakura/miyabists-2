using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
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
    internal class XuanmoJizhen : YixuanAtkCardBase
    {
        public XuanmoJizhen() : base(3, CardRarity.Rare, TargetType.AllEnemies)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar("HitCount",3),
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
                await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, 15, Owner.Creature, this);
            }

            await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, 20m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
