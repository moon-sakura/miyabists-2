using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class YixuanBlock:YixuanBlockCardBase
    {
        public YixuanBlock() : base(1, CardRarity.Basic, TargetType.Self, CardType.Skill)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/yixuanBlock.png";
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(5,ValueProp.Move),
            new DynamicVar(ThornsVarName,1),
        ];
        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
        }
    }
}
