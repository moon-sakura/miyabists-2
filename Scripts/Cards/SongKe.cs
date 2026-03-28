using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class SongKe : MiyabiPartnerCardBase
    {
        public SongKe() : base(3, CardRarity.Rare, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(DazeVarName, 15)
        ];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<MingCanXue>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.CheckSupportCost(3) != 0)
            {
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                reward1.AddKeyword(CardKeyword.Ethereal);
                reward1.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
                await CostSupporPoint(3);
            }

        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(15);
        }
    }
}
