using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class Princess : MiyabiCardBase
    {
        //protected override string ArtPath => "res://images/cards/SPxiaoye.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            MiyabiKeywords.OtherWorldFriends,
            CardKeyword.Exhaust
        ];

        public Princess()
            : base(0, CardType.Skill, CardRarity.Rare, TargetType.None)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            PrincessInop optionInop = base.Owner.Creature.CombatState?.CreateCard<PrincessInop>(base.Owner);
            PrincessYachiyo optionYachiyo = base.Owner.Creature.CombatState?.CreateCard<PrincessYachiyo>(base.Owner);
            PrincessYears option8000 = base.Owner.Creature.CombatState?.CreateCard<PrincessYears>(base.Owner);

            if (optionInop != null && optionYachiyo != null && option8000 != null)
            {
                List<CardModel> options = new List<CardModel> { optionInop, optionYachiyo, option8000 };

                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);

                await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, addedByPlayer: true);

                if (chosen is PrincessYears) 
                {
                    CardModel card = base.Owner.Creature.CombatState?.CreateCard<PrincessInoha>(base.Owner);
                    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: true);
                }
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
