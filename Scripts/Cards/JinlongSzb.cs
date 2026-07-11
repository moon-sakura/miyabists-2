using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class JinlongSzb : MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/jinlongSzb.png";

        public JinlongSzb() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<JinlongSzbPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            this.AddKeyword(CardKeyword.Innate);
            //EnergyCost.UpgradeBy(-1);
        }
    }
}
