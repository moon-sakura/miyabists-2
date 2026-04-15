using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class TianmiJingxia : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/tianmiJingxia.png";
        public TianmiJingxia() : base(1, CardRarity.Common, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Jingxia", 1)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<AnomalyBuildupPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal jx = 0;
            if (base.CheckSupportCost(1) != 0)
            {
                if(DynamicVars.TryGetValue("Jingxia", out DynamicVar v))
                {
                    jx = v.BaseValue;
                    v.BaseValue = 2;

                }
                await CostSupporPoint(1);
            }

            if (DynamicVars.TryGetValue("Jingxia", out var value))
            {
                await PowerCmd.Apply<TianmijxPower>(base.Owner.Creature, value.IntValue, base.Owner.Creature, this);
                value.BaseValue = jx;
            }

            
        }

        protected override void OnUpgrade()
        {
            this.AddKeyword(CardKeyword.Innate);
        }
    }
}
