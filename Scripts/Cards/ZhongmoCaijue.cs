using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class ZhongmoCaijue : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/zhongmoCaijue.png";

        public ZhongmoCaijue() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(10, ValueProp.Move),
            new DynamicVar(DazeVarName, 10),
            new DynamicVar(SupportVarName,3),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal damageor = DynamicVars.Damage.BaseValue;

            if (base.CheckSupportCost(DynamicVars[SupportVarName].IntValue) != 0)
            {
                damageor += 8;
                await CostSupporPoint(DynamicVars[SupportVarName].IntValue, choiceContext);
            }

            decimal daze = 0;
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v))
            {
                daze = v.BaseValue;
                //v.BaseValue = 0;
            }

            await base.OnPlay(choiceContext, cardPlay);

            if (damageor > 0)
            {
                await DamageCmd.Attack(damageor)
                    .FromCard(this, cardPlay)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            if (cardPlay.Target.HasPower<BreakPower>())
            {
                await PowerCmd.Remove<BreakPower>(cardPlay.Target);
                await PowerCmd.Apply<DazePower>(choiceContext, cardPlay.Target, daze, base.Owner.Creature, this);
            }
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        {
            if (cardSource == this && target.HasPower<BreakPower>())
            {
                return 3m;
            }

            return 1m;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(15);
        }
    }
}
