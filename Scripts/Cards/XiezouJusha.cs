using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
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
    [Pool(typeof(StatusCardPool))]
    internal class XiezouJusha : MiyabiPartnerCardBase
    {
        public XiezouJusha() : base(0, CardRarity.Token, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 6)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            await PowerCmd.Apply<DazeVulnPower>(cardPlay.Target, 15, base.Owner.Creature, this);
            
        }
        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
        }
    }
}
