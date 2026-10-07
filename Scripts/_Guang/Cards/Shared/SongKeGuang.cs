using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class SongKeGuang : SongKe
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromCard<MingCanXue>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //await base.OnPlay(choiceContext, cardPlay);
            await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, DynamicVars[DazeVarName], base.Owner.Creature);

            if (base.CheckSupportCost(DynamicVars[SupportVarName].IntValue) != 0)
            {
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                reward1.AddKeyword(CardKeyword.Ethereal);
                reward1.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner);
                await CostSupporPoint(DynamicVars[SupportVarName].IntValue, choiceContext);
            }

        }
    }
}
