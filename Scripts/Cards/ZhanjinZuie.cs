using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class ZhanjinZuie : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/zhanjinZuie.png";

        public ZhanjinZuie() : base(0, CardType.Power, CardRarity.Token, TargetType.Self, true) { }

        public override int MaxUpgradeLevel => 0;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Retain
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ZhanjinzePower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
        }

    }
}
