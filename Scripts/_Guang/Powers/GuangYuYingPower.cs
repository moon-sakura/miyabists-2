using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 光与影 - 能力。
    /// 造成伤害时获得等量的格挡。
    /// 被移除时，对随机敌人造成累计获得格挡的伤害。
    /// </summary>
    internal class GuangYuYingPower : ModPowerTemplate
    {
        /// <summary>累计获得的格挡（被移除时转化为伤害）</summary>
        //private decimal _accumulatedBlock;
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("BlockGained",0),
        ];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        /// <summary>图标上显示累计格挡，方便看清被移除时的伤害量</summary>
        public override int DisplayAmount => DynamicVars["BlockGained"].IntValue;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/guangYuying.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (!CombatManager.Instance.IsInProgress) return;
            if (dealer != Owner) return;
            if (result == null || result.TotalDamage <= 0) return;

            decimal amount = result.TotalDamage;
            decimal gained = await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null, false);

            DynamicVars["BlockGained"].BaseValue += gained;
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"被移除"
            if (!CombatManager.Instance.IsInProgress) return;
            if (oldOwner?.CombatState == null) return;
            if (DynamicVars["BlockGained"].BaseValue <= 0) return;

            List<Creature> enemies = oldOwner.CombatState.HittableEnemies.ToList();
            if (enemies.Count == 0) return;

            Creature target = oldOwner.CombatState.RunState.Rng.CombatTargets.NextItem<Creature>(enemies);

            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();
            await CreatureCmd.Damage(choiceContext, target, DynamicVars["BlockGained"].BaseValue, ValueProp.Unpowered, oldOwner);
        }
    }
}
