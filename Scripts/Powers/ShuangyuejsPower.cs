using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using Miyabists2.Scripts.Cards;

namespace Miyabists2.Scripts.Powers
{
    internal class ShuangyuejsPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/commonPowers.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<FrostFallPower>()
        ];

        //public override async  Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        //{
        //    if(side != base.Owner.Side) return;

        //    await PowerCmd.Apply<FrostFallPower>(base.Owner, Amount, null, null);
        //}

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card is not ShuangYue || cardPlay.Card.Owner.Creature != Owner)
                return;

            await CreatureCmd.GainBlock(Owner, 4m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, cardPlay);
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            await PowerCmd.Apply<FrostFallPower>(choiceContext, base.Owner, Amount, null, null);
        }
    }
}
