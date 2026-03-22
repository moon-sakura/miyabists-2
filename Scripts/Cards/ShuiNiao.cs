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
    internal class ShuiNiao : MiyabiBlockCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public override bool GainsBlock => false;

        public ShuiNiao() : base(1, CardRarity.Basic, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(0, ValueProp.Move),
            new DynamicVar(ParryVarName, 0),
            new DynamicVar(SlipperyVarName, 1)
        ];

        protected override void OnUpgrade()
        {
            // 升级增加 2 点护甲
            //DynamicVars.Block.UpgradeValueBy(2);

            base.EnergyCost.UpgradeBy(-1); // 升级后改为消耗 1 点能量

            // 如果需要升级 Parry 或 Slippery，可以在此添加逻辑
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
