using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Miyabists2.Scripts.Powers
{
    internal class BreakPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/powers/break.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<DazeVulnPower>()
        ];

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            //眩晕
            await CreatureCmd.Stun(base.Owner);
            //添加一回合失衡易伤50%
            await PowerCmd.Apply<DazeVulnPower>(base.Owner, 50m, null, null);

            foreach (Creature Player in base.CombatState.PlayerCreatures)
            {
                if (Player != null && Player.IsAlive && Player.HasPower<SupportPointPower>())
                {
                    await PowerCmd.Apply<SupportPointPower>(Player, 3, base.Owner, null);
                }
                //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if(side == base.Owner.Side)
            {
                //回合结束移除
                await PowerCmd.Remove<BreakPower>(base.Owner);
            }
        }
    }
}
