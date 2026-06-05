using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Potions
{
    [RegisterPotion(typeof(MiyabiPotionPool))]
    internal class WanquanZhunbeiPotion : MiyabiPotionBase
    {
        public override string? CustomImagePath => "res://images/potions/wanquanZhunbeiPotion.png";

        // 稀有度
        public override PotionRarity Rarity => PotionRarity.Uncommon;

        // 使用方式，CombatOnly表示只能在战斗中使用。
        public override PotionUsage Usage => PotionUsage.CombatOnly;

        // 目标类型
        public override TargetType TargetType => TargetType.Self;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [

        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<FrostFallPower>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
        ];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            await PowerCmd.Apply<MiyabiParryPower>(choiceContext, target, 2m, base.Owner.Creature, null);
            await PowerCmd.Apply<SupportPointPower>(choiceContext, target, 2m, base.Owner.Creature, null);
            await PowerCmd.Apply<FrostFallPower>(choiceContext, target, 2m, base.Owner.Creature, null);
            await PowerCmd.Apply<SlipperyPower>(choiceContext, target, 1m, base.Owner.Creature, null);
        }
    }
}
