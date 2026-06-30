using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class MojinYingxiao : YixuanBlockCardBase
    {
        public MojinYingxiao() : base(1, CardRarity.Common, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/mojinYingxiao.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(8, ValueProp.Move),
            new DynamicVar(ShannengVarName, 10),
            new DynamicVar(ThornsVarName, 2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            

            // 闪能10：获得荆棘
            await ShannengFunc(choiceContext, DynamicVars[ShannengVarName].IntValue, async () =>
            {
                // 抽一张卡
                var card = (await CardPileCmd.Draw(choiceContext, 1, Owner)).FirstOrDefault();

                if(card != null)
                {

                    if (card.Type == CardType.Skill)
                        await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);

                    if(card.Type == CardType.Attack)
                        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);
                }
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            DynamicVars[ThornsVarName].UpgradeValueBy(1);
        }
    }
}
