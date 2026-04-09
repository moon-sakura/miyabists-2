using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class ShuangyueJiashi : MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/shuangyueJiashi.png";
        public ShuangyueJiashi():base(3,CardType.Power,CardRarity.Common,TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("FrostFall", 1)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<FrostFallPower>(),
        ];

        protected override async  Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue("FrostFall", out DynamicVar v))
                await PowerCmd.Apply<ShuangyuejsPower>(Owner.Creature, v.BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            //EnergyCost.UpgradeBy(-1);
            if (base.DynamicVars.TryGetValue("FrostFall", out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
