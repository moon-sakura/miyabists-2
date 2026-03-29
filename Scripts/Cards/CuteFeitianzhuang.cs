using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class CuteFeitianzhuang : MiyabiPartnerCardBase
    {
        protected override string ArtPath => "res://images/cards/cuteFeitianzhuang.png";

        public CuteFeitianzhuang() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Skill) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(DazeVarName, 15)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            int add = 0;
            Creature target = cardPlay.Target;

            if (target.HasPower<AnomalyBuildupPower>())
                add += target.GetPowerAmount<AnomalyBuildupPower>() - 1;
            if (target.HasPower<AttributeAnomalyPower>())
                add += 3;
            if(target.HasPower<DisorderPower>())
                add += 5;

            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v))
                v.BaseValue += add * 5;

            if (base.CheckSupportCost(2) != 0)
            {
                v.BaseValue *= 1.2m;

                await CostSupporPoint(2);
            }


            base.OnPlay(choiceContext, cardPlay);
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(5);
        }
    }
}
