using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Miyabists2.Scripts.Cards
{
    internal class HanQue: MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public HanQue() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 4)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.Owner.Creature.HasPower<SlipperyPower>())
            {
                DynamicVars.Damage.BaseValue += 4;
            }
            await base.OnPlay(choiceContext, cardPlay);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
