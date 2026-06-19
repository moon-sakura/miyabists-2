using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class OneDennybooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/luckyboo.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, MinionSummonOptions options)
        {
            await base.OnSummon(choiceContext, owner, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<OneDennybooAct>(new ThrowingPlayerChoiceContext(), this.Creature, buffer, owner.Creature, options.Source);
        }
    }

    internal class OneDennybooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.None;
        public override string BigIconPath => "res://images/bangboo/relicMode/luckybooRelic.png";

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            foreach (Creature enemy in Owner.CombatState.Enemies.ToList())
            {
                int chance = 10 * DynamicVars["MAXUSE"].IntValue;
                if (!MiyabiFuncBase.GetIsTrue100(chance, Owner.PetOwner))
                    continue;

                var giveGold = new MoveState(
                    "MIYABI_ONEDENNYBOO_GIVE_GOLD",
                    async targets => await PlayerCmd.GainGold(1, Owner.PetOwner),
                    new UnknownIntent())
                {
                    FollowUpStateId = enemy.Monster.NextMove.StateId,
                    MustPerformOnceBeforeTransitioning = true
                };

                enemy.Monster.SetMoveImmediate(giveGold, true);
            }
        }
    }
}
