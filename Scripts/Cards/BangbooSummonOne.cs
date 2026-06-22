using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Service;
using MinionLib.Minion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class BangbooSummonOne : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooSummonOne() : base(0,CardType.Skill ,CardRarity.Uncommon,TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("summon",1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            for(int i = 0; i< DynamicVars["summon"].IntValue; i++)
            {
                await MiyabiCombatService.SummonBangbooRandom(choiceContext, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
            DynamicVars["summon"].UpgradeValueBy(1);
            base.OnUpgrade();
        }
    }
}
