using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class JipowangYipiePotion : MiyabiPotionBase
    {
        // 稀有度
        public override PotionRarity Rarity => PotionRarity.Rare;

        public override string? CustomPackedImagePath => "res://images/potions/jipowangYipiePotion.png";

        // 使用方式，CombatOnly表示只能在战斗中使用。
        public override PotionUsage Usage => PotionUsage.CombatOnly;

        // 目标类型
        public override TargetType TargetType => TargetType.AnyEnemy;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [

        ];

        public override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            if(target == null || target.HasPower<BreakPower>()) return;

            await PowerCmd.Apply<BreakPower>(choiceContext, target, 1m, base.Owner.Creature, null);
        }
    }
}
