using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Miyabists2.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class AllForHupowang : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/allForHupowang.png";
        public AllForHupowang() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy) { }

        //public new Texture2D Frame => ResourceLoader.Load<Texture2D>(ImageHelper.GetImagePath("atlases/card_atlas.sprites/beta.tres"), null, ResourceLoader.CacheMode.Reuse);

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int count = 0;
            List<CardModel> cardsIn = [];
            cardsIn.AddRange(base.Owner.PlayerCombatState.DrawPile.Cards.ToList());
            cardsIn.AddRange(base.Owner.PlayerCombatState.DiscardPile.Cards.ToList());
            cardsIn.AddRange(base.Owner.PlayerCombatState.Hand.Cards.ToList());
            foreach (CardModel c in cardsIn)
            {
                if(c.DynamicVars.TryGetValue("DAZE_POWER", out DynamicVar d) && d.BaseValue >= 15)
                {
                    count++;
                    await CardCmd.Exhaust(choiceContext, c);
                }
            }
            if(count >= 3)
            {
                await PowerCmd.Apply<BreakPower>(choiceContext, cardPlay.Target, 1m, base.Owner.Creature, this);
            }
            else
            {                
                await CardPileCmd.AddToCombatAndPreview<MenghuZhakaihua>(Owner.Creature, PileType.Draw, 3, null);
            }
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
