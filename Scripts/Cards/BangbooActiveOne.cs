using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MinionLib.Minion;
using MinionLib.Targeting;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class BangbooActiveOne : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooActiveOne() : base(0, CardType.Skill, CardRarity.Common, MinionTargetTypes.AnyMinion) { }

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

            if (act is ExplorebooAct)
            {
                await ((ExplorebooAct)act).ActEffect();
                return;
            }
            if(act is SharkbooAct)
            {
                await ((SharkbooAct)act).ActEffect();
                return;
            }
            

            ((MiyabiBangbooActBase)act).AddFree();
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            base.OnUpgrade();
        }
    }
}
