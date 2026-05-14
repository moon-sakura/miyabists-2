using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Char;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class MiyabiBangbooRelicBase : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.None;
        public override string PackedIconPath => "res://images/bangboo/relicMode/eousRelic.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;
        protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();

                var pet = await MinionCmd.AddMinion<EousBangboo>(choiceContext, Owner, new MinionSummonOptions(
                    MaxHp: 6m,                              // 血量
                    PrimaryStatAmount: 0m,                  // 主要参数（具体内容在随从的 OnSummon 里定义），还有次要参数等可以按需传入
                    Source: null,                           // 召唤来源（通常是这张牌）
                    Position: MinionPosition.Front));       // 站位（见后文，默认是前排）

                //NCombatRoom.Instance?.GetCreatureNode(pet)?.TrackBlockStatus(Owner.Creature);
            }
        }
    }
}
