using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.PotionPools;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Potions
{
    [RegisterPotion(typeof(SharedPotionPool))]
    internal class YanjiushengYaoyinPotion : MiyabiPotionBase
    {
        // 稀有度
        public override PotionRarity Rarity => PotionRarity.Rare;

        public override string? CustomImagePath => "res://images/potions/yanjiushengYaoyin.png";

        // 使用方式，CombatOnly表示只能在战斗中使用。
        public override PotionUsage Usage => PotionUsage.CombatOnly;

        // 目标类型
        public override TargetType TargetType => TargetType.AnyPlayer;

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            if(target.Player == null) return;

            decimal e = target.Player.PlayerCombatState.MaxEnergy - target.Player.PlayerCombatState.Energy;

            await PlayerCmd.GainEnergy(e, target.Player);
            await CardPileCmd.Draw(choiceContext, e, target.Player);
        }
    }
}
