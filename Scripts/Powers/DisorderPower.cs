using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Powers
{
    internal class DisorderPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/wenLuan.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<FrostFallPower>()
        ];

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            // 这里的 context 尝试从当前上下文中获取，如果没有则传 null
            //await CreatureCmd.Damage((PlayerChoiceContext)null, base.Owner, 50m, ValueProp.Unpowered, (Creature)null);
            //await DamageCmd.Attack(20m).Targeting(base.Owner).Execute(null);

            //PlayerChoiceContext choiceContext = new HookPlayerChoiceContext(applier.Player, applier.Player.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any);

            if (applier.IsMonster) return;

            foreach (Creature Player in base.CombatState.PlayerCreatures)
            {
                if (Player != null && Player.IsAlive && Player.HasPower<SupportPointPower>())
                {
                    await PowerCmd.Apply<FrostFallPower>(new ThrowingPlayerChoiceContext(), Player, 2, base.Owner, null);
                }
                //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
        }

        // 效果 2：下一击伤害 2 倍
        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (target == base.Owner && isValidMove)
            {
                return 2.0m;
            }
            return 1m;
        }

        // 效果 3：受到攻击后移除（实现“下一击”逻辑）
        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (result.TotalDamage > 0 && isValidMove && target == base.Owner)
            {
                // 触发一次后移除
                await PowerCmd.Remove(this);
            }
        }
    }
}
