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
    internal class QingXiaoJin : YixuanBlockCardBase
    {
        public QingXiaoJin() : base(2, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(12, ValueProp.Move),
            new DynamicVar(ThornsVarName, 3),
            new DynamicVar(VigorVarName, 3),
            new DynamicVar(ShannengVarName, 10),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            await ShannengFunc(choiceContext, DynamicVars[ShannengVarName].IntValue, async () =>
            {
                await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars[VigorVarName].IntValue, Owner.Creature, this);
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(4);
            DynamicVars[ThornsVarName].UpgradeValueBy(1);
            DynamicVars[VigorVarName].UpgradeValueBy(1);
        }
    }
}
