using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MinionLib.Targeting;
using Miyabists2.Scripts.Bangboo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class BangbooUseOnemore : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooUseOnemore() : base(1, CardType.Skill, CardRarity.Common, MinionTargetTypes.AnyMinion) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [

        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target is not { Monster: MiyabiBangbooBase } target) return;

            var act = target.Powers.Where(p => p is MiyabiBangbooActBase).FirstOrDefault();

            ((MiyabiBangbooActBase)act).MAXUSE++;            
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            base.OnUpgrade();
        }
    }
}
