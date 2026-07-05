using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class MoyingNingyun : YixuanAtkCardBase
    {
        public MoyingNingyun() : base(2, CardRarity.Common, TargetType.AllEnemies)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/moyingNingyun.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount", 2),
            new BlockVar(8, ValueProp.Move),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShufaZhi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["HitCount"].IntValue).FromCard(this, cardPlay)
                .Unblockable()
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            foreach(var enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, 10, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            DynamicVars[DazeVarName].UpgradeValueBy(2);
            DynamicVars.Block.UpgradeValueBy(2);
        }
    }
}
