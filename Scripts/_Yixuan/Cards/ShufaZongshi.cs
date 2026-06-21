using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class ShufaZongshi : YixuanCardBase
    {
        public ShufaZongshi() : base(2, CardType.Power,CardRarity.Rare, TargetType.Self)
        {
        }

        protected override string ArtPath => $"res://images/_YiXuan/cards/shufaZongshi.png";



        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("SHUFA_ZONGSHI_AMOUNT",1),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ShufaZongshiPower>(choiceContext, Owner.Creature, DynamicVars["SHUFA_ZONGSHI_AMOUNT"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["SHUFA_ZONGSHI_AMOUNT"].UpgradeValueBy(1);
        }
    }
}
