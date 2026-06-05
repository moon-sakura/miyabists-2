using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TongmuShuangxuePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/tongmuShuangxue.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckCard(cardPlay, choiceContext);
        }

        private async Task CheckCard(CardPlay cardPlay, PlayerChoiceContext choiceContext) 
        {
            if (cardPlay.Card.Owner.Creature != base.Owner
                || cardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                //|| cardPlay.Card.Type != CardType.Attack
                )
                return;
            await PowerCmd.Apply<SupportPointPower>(choiceContext, base.Owner,Amount,null,null);
        }
    }
}
