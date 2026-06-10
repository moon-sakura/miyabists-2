using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class QingmingZhenjiPo : YixuanAtkCardBase
    {
        public QingmingZhenjiPo() : base(2, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(17, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 8),
            new DynamicVar(ShufaVarName, 10),
            new DynamicVar(ShannengVarName, 20),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShufaZhi>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 施加术法值
            await PowerCmd.Apply<ShufaZhi>(choiceContext, cardPlay.Target, DynamicVars[ShufaVarName].IntValue, Owner.Creature, this);

            await ShannengFunc(new ThrowingPlayerChoiceContext(), DynamicVars[ShannengVarName].IntValue, async () =>
            {
                await PowerCmd.Apply<ShufaZhi>(choiceContext, cardPlay.Target, 10, Owner.Creature, this);
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
