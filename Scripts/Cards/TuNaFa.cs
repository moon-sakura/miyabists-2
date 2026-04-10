using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TuNaFa : MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/tunafa.png";
        public TuNaFa() : base(1,CardType.Power,CardRarity.Common,TargetType.Self) { }

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<FrostFallPower>(),
            HoverTipFactory.FromCard<FengHua>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<TunafaPower>(Owner.Creature, 1, Owner.Creature, this);

            //if (base.IsUpgraded)
            //{
            //    IEnumerable<CardModel> allCards = base.Owner.PlayerCombatState.AllCards;
            //    foreach (CardModel card in allCards)
            //    {
            //        if (card is FengHua)
            //        {
            //            card.EnergyCost.SetThisCombat(0);
            //        }
            //    }
            //}
        }

        //protected override void AddExtraArgsToDescription(LocString description)
        //{
        //    base.AddExtraArgsToDescription(description);
        //    description.Add("TunaUpgrade", "\n 本场战斗中卡组内所有风花费用变为0");
        //}

        protected override void OnUpgrade()
        {
            base.OnUpgrade();
            //AddExtraArgsToDescription(base.Description);
            EnergyCost.UpgradeBy(-1);
        }
    }
}
