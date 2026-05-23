using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    internal class LuckybooRelic : MiyabiBangbooRelicBase
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override string PackedIconPath => "res://images/bangboo/relicMode/luckybooRelic.png";

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber != 1) return;

            Flash();

            await base.AfterPlayerTurnStart(choiceContext, player);
            await MiyabiBangbooService.SummonBangboo<LuckybooBangboo>(Owner, 4m, MinionPosition.FrontUpper,null,1m);
        }
    }
}
