using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using MinionLib.Powers;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class PaperbooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/paperboo.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(choiceContext, owner, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<PaperbooAct>(new ThrowingPlayerChoiceContext(), this.Creature, buffer, owner.Creature, options.Source);

            await PowerCmd.Apply<MinionGuardianPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1m, owner.Creature, options.Source);
        }
    }

    internal class PaperbooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override string BigIconPath => "res://images/bangboo/relicMode/paperbooRelic.png";

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.GainBlock(Owner.PetOwner.Creature, 10m, ValueProp.Unpowered, null);
        }
    }
}
