using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class EarsLeft : MiyabiAttackCardBase
    {
        protected override string ArtPath => $"res://images/cards/earsAll.png";

        public EarsLeft() : base(1, CardRarity.Token, TargetType.AnyEnemy, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(10, ValueProp.Move),
            new DynamicVar(DazeVarName, 15),
            new CardsVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(5);
            DynamicVars[DazeVarName].UpgradeValueBy(5);
        }
    }
}
