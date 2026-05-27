using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Commands;
using MinionLib.Minion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class SharkbooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/sharkboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);
            await PowerCmd.Apply<SharkbooAct>(new ThrowingPlayerChoiceContext(), self, 1m, owner.Creature, options.Source);
        }
    }

    internal class SharkbooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.None;
        public override string BigIconPath => "res://images/bangboo/relicMode/sharkbooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Damage", 5m),
            new DynamicVar("AllDamage",0m),
            new DynamicVar ("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
        ];

        public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;
            await ActEffect(choiceContext, null);
        }

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            for (int i = 0; i < DynamicVars["MAXUSE"].IntValue; i++)
            {
                DynamicVars["AllDamage"].BaseValue += DynamicVars["Damage"].BaseValue;
            }
        }

        public override async Task OnCardActivate(PlayerChoiceContext choiceContext)
        {
            await ActEffect(choiceContext, null);
        }

        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner.PetOwner.Creature || dealer.IsPlayer) return;

            await MinionAnimCmd.PlayBumpAttackAsync(Owner, target);
            await CreatureCmd.Damage(choiceContext, dealer, DynamicVars["AllDamage"].BaseValue, ValueProp.Move, null, null);

            DynamicVars["AllDamage"].BaseValue = 0;
        }
    }
}
