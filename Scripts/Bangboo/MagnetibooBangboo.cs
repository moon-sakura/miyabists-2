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
    internal class MagnetibooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/magnetiboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options)
        {
            await base.OnSummon(owner, self, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<MagnetibooAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }
    }

    internal class MagnetibooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AllEnemies;
        public override string BigIconPath => "res://images/bangboo/relicMode/magnetibooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAXUSE", MAXUSE),
            new DynamicVar("AnoAmount", 1m),
            new DynamicVar("Used",0),
        ];

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            int anoAmount = DynamicVars["AnoAmount"].IntValue;
            foreach (var enemy in Owner.CombatState.Enemies)
            {
                await MiyabiCombatService.AddAnoBuildup(enemy, anoAmount, Owner, null, choiceContext);
            }
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;
            await ActEffect(choiceContext, null);
        }
    }
}