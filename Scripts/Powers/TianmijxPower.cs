using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TianmijxPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/tianmiJingxia.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AnomalyBuildupPower>()
        ];

        public override async Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;

            //for (int i = 0; i < Amount; i++)
            {
                foreach (Creature enemy in base.CombatState.Enemies)
                {
                    //await CreatureCmd.Damage(choiceContext, enemy, Amount * 3m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, (Creature)null);
                    if(enemy.IsAlive)
                    {
                        await MiyabiCombatService.AddAnoBuildup(enemy, Amount, base.Owner, null, choiceContext);
                    }
                }
            }
        }
    }
}
