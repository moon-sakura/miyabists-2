using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TongmuShuangxue : MiyabiCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public TongmuShuangxue():base(2,CardType.Power,CardRarity.Rare,TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<TongmuShuangxuePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            //await PowerCmd.Apply<TongmuShuangxuePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);

        }
    }
}
