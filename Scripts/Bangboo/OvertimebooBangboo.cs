using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class OvertimebooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/eous_bangboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<OvertimebooAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }
    }

    internal class OvertimebooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyPlayer;

        public override string BigIconPath => "res://images/bangboo/relicMode/overtimebooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            //new DynamicVar ("MAXUSE", 1),
            //new DynamicVar("TurnGap", 2m),
            new EnergyVar(1)
        ];

        public int UsedCount { get; set; } = 0;

        private bool used = false;

        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            //used = UsedCount >= DynamicVars["MAXUSE"].IntValue;
            //if (Owner.PetOwner.PlayerCombatState.Energy < 1 || used) return;

            await PlayerCmd.GainEnergy(1, target.Player);
            if(Owner.CurrentHp < 1)
            {
                await RelicCmd.Remove(MiyabiFuncBase.GetRelic<OvertimebooRelic>(Owner.PetOwner));
            }
            await CreatureCmd.Damage(choiceContext, base.Owner, 1m, ValueProp.Unpowered & ValueProp.Unblockable, base.Owner);
            //await MiyabiCombatService.AddDaze(choiceContext, target, DynamicVars["Daze"], base.Owner);
            //UsedCount++;
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            //UsedCount = 0;
            return base.AfterPlayerTurnStart(choiceContext, player);
        }
    }
}
