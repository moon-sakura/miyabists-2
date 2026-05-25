using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class ExcalibooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/excaliboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);

            //if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
            await PowerCmd.Apply<ExcalibooAct>(new ThrowingPlayerChoiceContext(), self, 1m, owner.Creature, options.Source);
        }
    }

    internal class ExcalibooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AllEnemies;
        public override string BigIconPath => "res://images/bangboo/relicMode/excalibooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
            new DynamicVar("Charged",0),
            new DamageVar(25m, ValueProp.Move),
        ];

        //public int UsedCount { get; set; } = 0;

        private bool used = false;

        //public bool isFree { get; set; } = false;

        public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            await ActEffect(choiceContext);
        }


        public async Task ActEffect(PlayerChoiceContext choiceContext)
        {
            DynamicVars["Charged"].BaseValue += MAXUSE;

            if (DynamicVars["Used"].BaseValue >= 3 || MiyabiFuncBase.GetIsTrue100(20,Owner.PetOwner))
            {
                await MinionAnimCmd.PlayBumpAttackAsync(Owner, Owner.CombatState.Enemies.FirstOrDefault());
                await CreatureCmd.Damage(choiceContext, Owner.CombatState.Enemies, DynamicVars.Damage, Owner);
                DynamicVars["Charged"].BaseValue = 0;
            }
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            return base.AfterPlayerTurnStart(choiceContext, player);
        }
    }
}
