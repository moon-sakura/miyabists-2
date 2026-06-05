using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class FrostFirePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromKeyword(MiyabiKeywords.LieShuang)
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar ("Limit", 50),
        ];

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            decimal limit = MiyabiCombatService.GetFrostFireLimit();
            DynamicVars["Limit"].BaseValue = limit * 100;
            return base.AfterApplied(applier, cardSource);
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner)
            {
                return 1m;
            }
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (!isValidMove) return 1m;

            if (cardSource == null)
                return 1m;

            if(!cardSource.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang))
                return 1m;

            decimal limit = MiyabiCombatService.GetFrostFireLimit();
            //DynamicVars["Limit"].BaseValue = limit * 100;

            //烈霜伤害+5%（效果上限150%，层数无上限）
            decimal damageIncrease = base.Amount * 0.05m;
            if (damageIncrease > limit)
            {
                damageIncrease = limit;
            }

            return 1m + damageIncrease;
        }
    }
}
