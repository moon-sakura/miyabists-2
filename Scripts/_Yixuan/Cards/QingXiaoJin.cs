using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Cards.NoneShow;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Service;
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

        protected override string ArtPath => "res://images/_YiXuan/cards/qingxiaoJin.png";

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(12, ValueProp.Move),
            new DynamicVar(ThornsVarName, 3),
            new DynamicVar(VigorVarName, 3),
            new DynamicVar(ShannengVarName, 10),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            await ShannengFunc(choiceContext, DynamicVars[ShannengVarName].IntValue, async () =>
            {
                // 荆棘或活力二选一
                await MiyabiCombatService.ChooseResYi(choiceContext, base.Owner, new Dictionary<Type, (int, Func<PlayerChoiceContext, Task>)>
                {
                    { typeof(ThornsChoice), (DynamicVars[ThornsVarName].IntValue, async ctx => {
                        await PowerCmd.Apply<ThornsPower>(ctx, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);
                    })},
                    { typeof(VigorChoice), (DynamicVars[VigorVarName].IntValue, async ctx => {
                        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, DynamicVars[VigorVarName].IntValue, Owner.Creature, this);
                    })},
                });
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
