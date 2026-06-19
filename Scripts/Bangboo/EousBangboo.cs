using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
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
    internal class EousBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/eous_bangboo.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(choiceContext, owner, options);

            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<EousAct>(new ThrowingPlayerChoiceContext(), this.Creature, buffer, owner.Creature, options.Source);
        }

    }

    internal class EousAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override string BigIconPath => "res://images/bangboo/relicMode/eousRelic.png";

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.GainBlock(target, 6m, ValueProp.Unpowered, null);
        }
    }
}
