using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Buffers;

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
            //HoverTipFactory.FromPower<DazeVulnPower>()
        ];


        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            PlayerChoiceContext choiceContext = new HookPlayerChoiceContext(applier.Player, applier.Player.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any);

            await CreatureCmd.Stun(base.Owner);
                //添加一回合失衡易伤50%
                //if(IsAnyHasTBPZJQ())
                //{ 
                //    await PowerCmd.Apply<DazeVulnPower>(base.Owner, 80m, null, null); 
                //}
                //else
            await PowerCmd.Apply<DazeVulnPower>(choiceContext,base.Owner, 50m, null, null);


            foreach (Creature Player in base.CombatState.PlayerCreatures)
            {
                if (Player != null && Player.IsAlive && Player.HasPower<SupportPointPower>())
                {
                    await PowerCmd.Apply<SupportPointPower>(choiceContext, Player, 3, base.Owner, null);
                }
                //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
        }

        //private bool IsAnyHasTBPZJQ()
        //{
        //    foreach (Creature Player in base.CombatState.PlayerCreatures)
        //    {
        //        if (Player != null && Player.IsAlive && Player.HasPower<TebiePzjqPower>())
        //        {
        //            return true;
        //        }
        //        //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
        //    }
        //    return false;
        //}

        //public override bool ShouldPlay(CardModel card, AutoPlayType _)
        //{
        //    if (card.Owner.Creature != base.Owner || !base.Owner.IsPlayer)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if(side == base.Owner.Side)
            {
                //回合结束移除
                await PowerCmd.Remove(this);
            }
        }
    }
}
