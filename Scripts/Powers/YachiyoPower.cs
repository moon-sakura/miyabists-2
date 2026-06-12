using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class YachiyoPower : ModPowerTemplate, IModRightClickablePower
    {
        private class Data
        {
            public CardModel? selectedCard;
        }

        public string BigIconPath => "res://images/powers/yachiyo.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private const string _cardKey = "Card";

        public override PowerType Type => PowerType.Buff;

        //public override bool  IsInstanced => true;

        public override PowerStackType StackType => PowerStackType.Counter;

        protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];

        protected override object InitInternalData()
        {
            return new Data();
        }

        //public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
        //{
        //    if (player == base.Owner.Player)
        //    {
        //        CardModel card = GetInternalData<Data>().selectedCard;
        //        for (int i = 0; i < base.Amount; i++)
        //        {
        //            CardModel card2 = card.CreateClone();
        //            card2.SetToFreeThisTurn();
        //            await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Hand, Owner.Player);
        //        }
        //        await PowerCmd.Remove(this);
        //    }
        //}

        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            return Amount > 0;
        }

        // 右键执行（多人下会在所有客户端同步执行）
        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            CardModel card = GetInternalData<Data>().selectedCard;
            for (int i = 0; i < base.Amount; i++)
            {
                CardModel card2 = card.CreateClone();
                card2.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Hand, Owner.Player);
            }
            await PowerCmd.Remove(this);
        }

        public void SetSelectedCard(CardModel card)
        {
            GetInternalData<Data>().selectedCard = card.CreateClone();
            ((StringVar)base.DynamicVars["Card"]).StringValue = card.Title;
        }
    }
}
