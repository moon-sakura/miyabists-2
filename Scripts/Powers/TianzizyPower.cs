using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class TianzizyPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/miyabiFull.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => 
        [
            HoverTipFactory.FromCard<ShuangYueSp>(),
            HoverTipFactory.FromCard<ShuangYue>()
        ];

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if(cardPlay.Card.Owner.Creature != base.Owner || !(cardPlay.Card is ShuangYue)) return;

            bool flag = true;

            ShuangYueSp reward1 = base.Owner.CombatState.CreateCard<ShuangYueSp>(base.Owner.Player);
            int targetLevel = cardPlay.Card.CurrentUpgradeLevel;
            for (int i = 0; i < targetLevel; i++)
            {
                reward1.UpgradeInternal();
            }
            for(int i = 0; i < this.Amount; i++)
                await CardCmd.AutoPlay(context, reward1, cardPlay.Target, AutoPlayType.Default, skipXCapture: false, !flag);

            flag = false;
        }
    }
}
