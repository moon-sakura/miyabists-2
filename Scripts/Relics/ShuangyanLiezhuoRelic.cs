using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class ShuangyanLiezhuoRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => "res://images/relics/shuangyanLiezhuo.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromCard<JixianShiyu>(),
            //HoverTipFactory.FromPower<FrostFallPower>(),
            //HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner || base.Owner.Creature.CombatState.RoundNumber != 1) return;
            MiyabiCombatService.SetFrostFireLimit(1m);
            MiyabiCombatService.SetCanAddWhenFire(true);
        }

        public override Task AfterCombatEnd(CombatRoom room)
        {
            MiyabiCombatService.ResetFrostFireLimit();
            MiyabiCombatService.ResetCanAddWhenFire();
            return base.AfterCombatEnd(room);
        }
    }
}
