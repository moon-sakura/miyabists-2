using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class ShimengKongxiangPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/shimengKongxiang.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("HpLossPercent", 10),
            new DynamicVar("DecibelAmount", 1),
        ];

        // 每回合开始时失去10%生命值
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            decimal hpLoss = Owner.MaxHp * DynamicVars["HpLossPercent"].BaseValue / 100m;
            await CreatureCmd.Damage(choiceContext, Owner, hpLoss, ValueProp.Unpowered | ValueProp.Unblockable, Owner);
        }

        // 命破伤害次数+1（通过伤害倍率模拟：(N+1)/N）
        public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
        {
            if(attack.Attacker == Owner && attack.DamageProps.IsPoweredAttack() && attack.DamageProps.HasFlag(ValueProp.Unblockable))
            {
                return hitCount + Amount;
            }

            return base.ModifyAttackHitCount(attack, hitCount);
        }
        //public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        //{
        //    if (dealer != Owner || cardSource == null)
        //        return 1m;

        //    if (cardSource.CanonicalKeywords.Any(k => k == MiyabiKeywords.Mingpo)
        //        && cardSource.Owner.Creature == Owner)
        //    {
        //        int hitCount = 1;
        //        if (cardSource.DynamicVars.TryGetValue("HitCount", out var hc))
        //            hitCount = Math.Max(1, hc.IntValue);
        //        return (decimal)(hitCount + 1) / hitCount;
        //    }
        //    return 1m;
        //}

        // 每次失去生命值时获得1点喧响值
        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner) return;
            if (result.TotalDamage > 0)
            {
                await MiyabiCombatService.AddDecible(Owner.Player, Amount * DynamicVars["DecibelAmount"].IntValue);
            }
        }
    }
}
