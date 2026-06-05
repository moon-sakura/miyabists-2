
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class MingfuwgPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/mingfuWange.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<XiezouJusha>()];
        
        //public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        //{
        //    if(side != base.Owner.Side) return;
        //    CardModel reward1 = base.Owner.CombatState.CreateCard<XiezouJusha>(base.Owner.Player);
        //    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner.Player, CardPilePosition.Random);

        //    await PowerCmd.TickDownDuration(this);
        //}

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            CardModel reward1 = base.Owner.CombatState.CreateCard<XiezouJusha>(base.Owner.Player);
            await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner.Player, CardPilePosition.Random);

            await PowerCmd.TickDownDuration(this);
        }
    }
}
