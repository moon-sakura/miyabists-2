using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;

namespace Miyabists2.Scripts.Powers
{
    internal class ZhanjinzePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/notailFull.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
        ];

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = !props.HasFlag(ValueProp.Unpowered);
            if (dealer == base.Owner && isValidMove)
            {
                return 2m;
            }
            return 1m;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if(side == Owner.Side)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}
