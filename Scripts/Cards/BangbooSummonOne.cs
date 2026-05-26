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
    internal class BangbooSummonOne : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooSummonOne() : base(1,CardType.Skill ,CardRarity.Uncommon,TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int result = MiyabiFuncBase.RadomInt(0, 11, Owner);

            if (result == 0)
            {
                await MiyabiBangbooService.SummonBangboo<EousBangboo>(Owner, 6m, MinionPosition.Back, null, 1m);
            }
            else if (result == 1)
            {
                await MiyabiBangbooService.SummonBangboo<LuckybooBangboo>(Owner, 4m, MinionPosition.FrontUpper, null, 1m);
            }
            else if (result == 2)
            {
                await MiyabiBangbooService.SummonBangboo<ExplorebooBangboo>(Owner, 4m, MinionPosition.Back);
            }
            else if (result == 3)
            {
                await MiyabiBangbooService.SummonBangboo<SumobooBangboo>(Owner, 4m, MinionPosition.Front, null, 1m);
            }
            else if (result == 4)
            {
                await MiyabiBangbooService.SummonBangboo<PaperbooBangboo>(Owner, 15m, MinionPosition.Front, null, 1m);
            }
            else if (result == 5)
            {
                await MiyabiBangbooService.SummonBangboo<OvertimebooBangboo>(Owner, 3m, MinionPosition.Back, null, 1m);
            }
            else if(result == 6)
            {
                await MiyabiBangbooService.SummonBangboo<SharkbooBangboo>(Owner, 8m, MinionPosition.FrontUpper, null, 1m);
            }
            else if(result == 7)
            {
                await MiyabiBangbooService.SummonBangboo<ExcalibooBangboo>(Owner, 8m, MinionPosition.BackUpper);
            }
            else if(result == 8)
            {
                await MiyabiBangbooService.SummonBangboo<AgentBangboo>(Owner, 10m, MinionPosition.FrontUpper);
            }
            else if(result == 9)
            {
                await MiyabiBangbooService.SummonBangboo<MagnetibooBangboo>(Owner, 8m, MinionPosition.FrontUpper, null, 1m);
            }
            else if(result == 10)
            {
                await MiyabiBangbooService.SummonBangboo<OneDennybooBangboo>(Owner, 4m, MinionPosition.FrontUpper, null, 1m);
            }
            
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
            base.OnUpgrade();
        }
    }
}
