using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class SongKeYixuan : SongKe
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromCard<QingmingYunying>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
            {
                await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, dazeVar, base.Owner.Creature);
            }

            if (base.CheckSupportCost(DynamicVars[SupportVarName].IntValue) != 0)
            {
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<QingmingYunying>(base.Owner.Creature.Player);
                reward1.AddKeyword(CardKeyword.Ethereal);
                reward1.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                await CostSupporPoint(DynamicVars[SupportVarName].IntValue, choiceContext);
            }
        }
    }
}
