using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class LingyunPo : YixuanAtkCardBase
    {
        public LingyunPo() : base(1, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(7, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar(VigorVarName, 2),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars[VigorVarName].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(VigorVarName, out DynamicVar vigorVar)) vigorVar.UpgradeValueBy(1);
        }
    }
}
