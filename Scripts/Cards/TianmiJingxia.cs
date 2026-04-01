using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    internal class TianmiJingxia : MiyabiPartnerCardBase
    {
        public TianmiJingxia() : base(1, CardRarity.Common, TargetType.Self, CardType.Power) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<TianmijxPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);

            if (base.CheckSupportCost(1) != 0)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        await PowerCmd.Apply<AnomalyBuildupPower>(Enemy, 1, base.Owner.Creature, this); ;
                    }
                    //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
                }
                await CostSupporPoint(1);
            }
        }

        protected override void OnUpgrade()
        {
            this.AddKeyword(CardKeyword.Innate);
        }
    }
}
