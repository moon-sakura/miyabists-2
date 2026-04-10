using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    internal class ZhongmuYanchu : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/zhongmuYanchu.png";

        public ZhongmuYanchu() : base(2, CardRarity.Rare, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 10),
            new DynamicVar(AnomalyBuildupVarName, 3)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<AnomalyBuildupPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .WithHitCount(3)
                    .Execute(choiceContext);
            }

            if (base.CheckSupportCost(3) != 0 && cardPlay.Target.IsAlive)
            {
                await PowerCmd.Apply<ZhongmuycPower>(cardPlay.Target, 1, cardPlay.Target, this);
                await CostSupporPoint(3);
            }
        }

        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(3);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(15);
            if (base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out DynamicVar a)) a.UpgradeValueBy(1);

        }
    }
}
