using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    internal class ExcalibooRelic : MiyabiBangbooRelicBase
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override string PackedIconPath => "res://images/bangboo/relicMode/excalibooRelic.png";

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber != 1) return;

            Flash();

            await base.AfterPlayerTurnStart(choiceContext, player);
            await MiyabiBangbooService.SummonBangboo<ExcalibooBangboo>(Owner, 8m, MinionPosition.BackUpper);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<ExcalibooAct>(),
        ];
    }
}
