using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    internal class ZhongmoCaijue : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public ZhongmoCaijue() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(10, ValueProp.Move),
            new DynamicVar(DazeVarName, 10),
            new BlockVar(0, ValueProp.Move)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.CheckSupportCost(3) != 0)
            {
                DynamicVars.Damage.BaseValue += 8;
                await CostSupporPoint(3);
            }
            if(cardPlay.Target.HasPower<BreakPower>()) DynamicVars.Damage.BaseValue *= 3;

            decimal daze = 0;
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v))
            {
                daze = v.BaseValue;
                v.BaseValue = 0;
            }

            await base.OnPlay(choiceContext, cardPlay);

            if (cardPlay.Target.HasPower<BreakPower>())
            {
                await PowerCmd.Remove<BreakPower>(cardPlay.Target);
                await PowerCmd.Remove<DazeVulnPower>(cardPlay.Target);
                await PowerCmd.Apply<DazePower>(cardPlay.Target, daze, base.Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(15);
        }
    }
}
