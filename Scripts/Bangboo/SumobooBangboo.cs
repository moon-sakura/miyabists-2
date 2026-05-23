using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class SumobooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/sumoboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<SumobooAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }
    }

    internal class SumobooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyEnemy;

        public override string BigIconPath => "res://images/bangboo/relicMode/sumobooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar ("MAXUSE", MAXUSE),
            new DynamicVar("Daze", 25m),
            new DynamicVar("Used",0),
        ];

        //public int UsedCount { get; set; } = 0;

        private bool used = false;

        //public bool isFree { get; set; } = false;

        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            DynamicVars["MAXUSE"].BaseValue = MAXUSE;
            DynamicVars["Used"].BaseValue = UsedCount;

            used = UsedCount >= DynamicVars["MAXUSE"].IntValue;
            if ((Owner.PetOwner.PlayerCombatState.Energy < 1 || used) && !isFree) return;

            await ActEffect(choiceContext,target);
            if (!isFree)
            {
                await ActCost();
            }
            isFree = false;
        }

        public async Task ActCost()
        {
            await PlayerCmd.LoseEnergy(1, Owner.PetOwner);
            UsedCount++;
            DynamicVars["Used"].BaseValue = UsedCount;
            isFree = false;
        }

        public async Task ActEffect(PlayerChoiceContext choiceContext,Creature target)
        {
            await MiyabiCombatService.AddDaze(choiceContext, target, DynamicVars["Daze"], base.Owner);
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            UsedCount = 0;
            DynamicVars["Used"].BaseValue = UsedCount;
            return base.AfterPlayerTurnStart(choiceContext, player);
        }
    }
}
