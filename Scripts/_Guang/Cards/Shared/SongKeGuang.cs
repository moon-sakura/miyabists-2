using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts._Guang.Powers;
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
            HoverTipFactory.FromCard<ZhuYunJingTing>(),
            HoverTipFactory.FromCard<ZhanWangKaiTian>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //await base.OnPlay(choiceContext, cardPlay);
            await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, DynamicVars[DazeVarName], base.Owner.Creature);

            if (base.CheckSupportCost(DynamicVars[SupportVarName].IntValue) != 0)
            {
                // 明心镜状态下送的是《斩妄开天》，否则送《逐云惊霆》
                CardModel reward1 = base.Owner.Creature.HasPower<MingxinJingPower>()
                    ? base.Owner.Creature.CombatState.CreateCard<ZhanWangKaiTian>(base.Owner.Creature.Player)
                    : base.Owner.Creature.CombatState.CreateCard<ZhuYunJingTing>(base.Owner.Creature.Player);
                reward1.AddKeyword(CardKeyword.Ethereal);
                reward1.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner);
                await CostSupporPoint(DynamicVars[SupportVarName].IntValue, choiceContext);
            }

        }
    }
}
