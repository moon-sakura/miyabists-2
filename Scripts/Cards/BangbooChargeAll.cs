using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MinionLib.Targeting;
using Miyabists2.Scripts.Bangboo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class BangbooChargeAll : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooChargeAll() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [

        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            foreach(var bangboo in Owner.Creature.Pets)
            {
                if(bangboo.Monster is MiyabiBangbooBase)
                {
                    var act = bangboo.Powers.Where(p => p is MiyabiBangbooActBase).FirstOrDefault();
                    int i = ((MiyabiBangbooActBase)act).UsedCount;
                    if (i > 0)
                        ((MiyabiBangbooActBase)act).UsedCount-- ;
                }
            }
        }

        protected override void OnUpgrade()
        {
            //RemoveKeyword(CardKeyword.Exhaust);
            EnergyCost.UpgradeBy(-1);
            base.OnUpgrade();
        }
    }
}
