using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class ZuihuaYueyunzhuan : MiyabiPartnerCardBase
    {
        public ZuihuaYueyunzhuan() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(1, ValueProp.Move),
            new DynamicVar(DazeVarName, 15),
            new DynamicVar("Jifu", 5),
            new DynamicVar("HitCount", 5),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),

            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .WithHitCount(DynamicVars["HitCount"].IntValue)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            if(!cardPlay.Target.HasPower<BreakPower>() && base.DynamicVars.TryGetValue("Jifu", out DynamicVar u))
            {

                if (base.CheckSupportCost(1) != 0)
                {
                    u.BaseValue += 3;
                    await CostSupporPoint(1);
                }
                if (base.Owner.Creature.HasPower<SlipperyPower>())
                {
                    u.BaseValue += 2;
                }
                await PowerCmd.Apply<JifuPower>(cardPlay.Target, u.IntValue, base.Owner.Creature, this);
            }

        }

        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(5);

        }
    }
}
