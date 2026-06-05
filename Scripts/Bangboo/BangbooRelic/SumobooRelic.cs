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
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class SumobooRelic : MiyabiBangbooRelicBase
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override string PackedIconPath => "res://images/bangboo/relicMode/sumobooRelic.png";

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber != 1) return;

            Flash();

            await base.AfterPlayerTurnStart(choiceContext, player);
            await MiyabiBangbooService.SummonBangboo<SumobooBangboo>(Owner, 6m, MinionPosition.Front,null,1m);
        }

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SumobooAct>(),
        ];
    }
}
