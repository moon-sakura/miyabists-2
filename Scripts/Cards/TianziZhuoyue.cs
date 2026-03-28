using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TianziZhuoyue : MiyabiCardBase
    {
        public TianziZhuoyue() : base(4, CardType.Power, CardRarity.Rare, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<TianzizyPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<ShuangYueSp>()];

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
