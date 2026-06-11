using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class TieshanZhenmaikao : YixuanPartnerCardBase
    {
        public TieshanZhenmaikao() : base(2, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/tieshanZhenmaikao.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MingpoCount", 3),
            new DynamicVar("MingpoBonus", 20),
            new DynamicVar(SupportVarName, 2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.Mingpo),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int fee = 0;

            // 支援点数2：抽1张卡，下一张攻击卡费用-1
            await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
            {
                fee = 1;
                await CardPileCmd.Draw(choiceContext, 1, Owner);
            });

            // 接下来3张命破伤害卡伤害+20%
            await PowerCmd.Apply<TieshanZhenmaikaoPower>(choiceContext, Owner.Creature, DynamicVars["MingpoCount"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
