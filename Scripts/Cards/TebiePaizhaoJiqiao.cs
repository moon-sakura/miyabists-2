using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TebiePaizhaoJiqiao : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/tebiePaizhaoJiqiao.png";
        public TebiePaizhaoJiqiao() : base(1, CardRarity.Uncommon, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DynamicVar("DazeVuln", 15),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            (await PowerCmd.Apply<TebiePzjqPower>(base.Owner.Creature, 1, base.Owner.Creature, this)).SetDazeVuln(DynamicVars["DazeVuln"].IntValue);

        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("DazeVuln", out DynamicVar v)) v.UpgradeValueBy(15);
        }
    }
}
