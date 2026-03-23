using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(MiyabiCardPool))]
    internal class MiyabiBlock : MiyabiBlockCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public MiyabiBlock() : base(1, CardRarity.Basic, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(4, ValueProp.Move),
            new DynamicVar(ParryVarName, 1),
            new DynamicVar(SlipperyVarName, 0)
        ];

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };


        protected override void OnUpgrade()
        {
            // 升级增加 2 点护甲
            DynamicVars.Block.UpgradeValueBy(2);

            // 如果需要升级 Parry 或 Slippery，可以在此添加逻辑
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
