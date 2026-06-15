using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
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
        protected override string VisualsPath => "res://scenes/bangboo/overtimeboo.tscn";

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
            new DynamicVar ("MAXUSE", MAXUSE),
            new DynamicVar("Used", 0m),
            new EnergyVar(1)
        ];

        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            await ActEffect(choiceContext, target);
            if (isFree < 1)
                await ActCost();
            isFree--;
            if (isFree < 0) isFree = 0;
        }

        public override async Task ActCost()
        {
            if (Owner.CurrentHp <= 1){
                var relic = MiyabiFuncBase.GetRelic<OvertimebooRelic>(Owner.PetOwner);
                if (relic != null)
                    await RelicCmd.Remove(relic);
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, 1m, ValueProp.Unblockable, base.Owner);
        }

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            await PlayerCmd.GainEnergy(1, target.Player);
        }
    }
}
