using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    /// <summary>
    /// 无拘剑势能力：每次使用攻击卡时，对随机敌人造成4点伤害
    /// </summary>
    internal class WujuJianshiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/wujuJianshi.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;
            if (cardPlay.Card.Type != CardType.Attack) return;

            var enemy = Owner.CombatState.RunState.Rng.CombatTargets.NextItem<Creature>(Owner.CombatState.HittableEnemies);

            // 对随机敌人造成4点伤害
            await CreatureCmd.Damage(choiceContext, enemy, Amount, ValueProp.Unpowered, (Creature)null);
        }
    }
}
