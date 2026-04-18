using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class WeimingDoudaoshu : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/commonCards.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.OtherWorldFriends];

        public WeimingDoudaoshu()
            : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Daze", 25)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<HuaCi>(),
            HoverTipFactory.FromPower<BreakPlayerPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            (await PowerCmd.Apply<WeimingddsPower>(base.Owner.Creature, 1m, Owner.Creature, this)).SetDazeAdd(DynamicVars["Daze"].IntValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Daze"].BaseValue -= 5;
            AddKeyword(CardKeyword.Innate);
        }
    }
}
