using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class ExplorebooBangboo: MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/exploreboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);
            await PowerCmd.Apply<ExplorebooAct>(new ThrowingPlayerChoiceContext(), self, 1m, owner.Creature, options.Source);
        }
    }

    internal class ExplorebooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.None;

        public string BigIconPath => "res://images/bangboo/relicMode/explorebooRelic.png";
        public string BigBetaIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Gold", 0m),
            new DynamicVar("MaxGold",25m)
        ];

        public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            decimal gold = 5m * Owner.PetOwner.Creature.CombatState.RoundNumber > DynamicVars["MaxGold"].BaseValue ? DynamicVars["MaxGold"].BaseValue : 5m * Owner.PetOwner.Creature.CombatState.RoundNumber;
            DynamicVars["Gold"].BaseValue += gold;
        }

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            await PlayerCmd.GainGold(DynamicVars["Gold"].BaseValue, Owner.PetOwner);
        }
    }
}
