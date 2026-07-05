using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Commands;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MegaCrit.Sts2.Core.Models.Monsters.KnowledgeDemon;

namespace Miyabists2.Scripts.Powers
{
    internal class SummonMonsterPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/summonMonster.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
        {
            if (target != base.Owner.PetOwner?.Creature)
            {
                return target;
            }
            if (base.Owner.IsDead)
            {
                return target;
            }
            if (!props.IsPoweredAttack_())
            {
                return target;
            }
            return base.Owner;
        }

        public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner)
            {
                return amount;
            }
            if (base.Owner.IsDead)
            {
                return amount;
            }
            if (!props.IsPoweredAttack_())
            {
                return amount;
            }
            return amount - base.Owner.DamageBlockInternal(amount, props);
        }

        //public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
        //{
        //    if (player == base.Owner.PetOwner)
        //    {
        //        ArgumentNullException.ThrowIfNull(base.Owner.PetOwner, "base.Owner.PetOwner");
        //        ArgumentNullException.ThrowIfNull(base.Owner.Monster, "base.Owner.Monster");
        //        await MinionMove();
        //    }
        //}

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            MonsterModel monster = Owner.Monster;

            return base.AfterApplied(applier, cardSource);
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if(side == Owner.Side)
                await MinionMove();
        }

        public async Task MinionMove()
        {
            MonsterModel monster = Owner.Monster;

            IReadOnlyList<Creature> targets =
                Owner.PetOwner.Creature.CombatState.HittableEnemies;

            //GD.Print($"[SummonMonsterPower] MoveState is null?: {monster.MoveStateMachine == null}");

            //GD.Print($"[SummonMonsterPower] Next Move is : {monster.NextMove.Id}");
            monster.RollMove(targets);


            //GD.Print($"[SummonMonsterPower] Next Move is : {monster.NextMove.Id}");

            await MiyabiFuncBase.PerformPetMonsterMove(Owner);

        }
    }
}
