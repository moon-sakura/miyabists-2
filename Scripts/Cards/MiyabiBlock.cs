using STS2RitsuLib.Interop.AutoRegistration;

using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class MiyabiBlock : MiyabiBlockCardBase
    {
        protected override string ArtPath => $"res://images/cards/miyabiBlock.png";

        public MiyabiBlock() : base(1, CardRarity.Basic, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(Block(), ValueProp.Move),
            new DynamicVar(ParryVarName, 1),
            new DynamicVar(SlipperyVarName, 0)
        ];

        public override void AfterCreated()
        {
            DynamicVars.Block.BaseValue = Block();
            base.AfterCreated();
        }

        decimal Block() => (int)MiyabiModConfig.CombatHardSelected >= 4 ? 6m : 4m;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
        ];

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

        


        protected override void OnUpgrade()
        {
            // 升级增加 2 点护�?            DynamicVars.Block.UpgradeValueBy(2);

            // 如果需要升�?Parry �?Slippery，可以在此添加逻辑
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
