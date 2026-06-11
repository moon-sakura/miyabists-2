using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QingxiaoTongyou : YixuanCardBase
    {
        public QingxiaoTongyou() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar(VigorVarName, 2),
            new DynamicVar(ThornsVarName, 2),
            new DynamicVar(ShannengVarName, 5),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var power = await PowerCmd.Apply<QingxiaoTongyouPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            power.SetAmounts(
                DynamicVars[VigorVarName].IntValue,
                DynamicVars[ThornsVarName].IntValue,
                DynamicVars[ShannengVarName].IntValue
            );
        }

        protected override void OnUpgrade()
        {
            DynamicVars[VigorVarName].UpgradeValueBy(1);
            DynamicVars[ThornsVarName].UpgradeValueBy(1);
            DynamicVars[ShannengVarName].UpgradeValueBy(5);
        }
    }
}
