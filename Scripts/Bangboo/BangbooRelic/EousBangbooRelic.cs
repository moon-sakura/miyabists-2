using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    
    internal class EousBangbooRelic : MiyabiBangbooRelicBase
    {
        public override string PackedIconPath => "res://images/bangboo/relicMode/eousRelic.png";
        private const int _roomCount = 3;
        private int _timesUsed;

        public override bool IsUsedUp => TimesUsed >= 3;
        public override bool ShowCounter => !IsUsedUp;
        public override int DisplayAmount => 3 - TimesUsed;


        [SavedProperty]
        public int TimesUsed
        {
            get
            {
                return _timesUsed;
            }
            set
            {
                AssertMutable();
                _timesUsed = value;
                base.DynamicVars["Rooms"].BaseValue = 3 - _timesUsed;
                InvokeDisplayAmountChanged();
                CheckIfUsedUp();
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Rooms", 3m)];


        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();

                await MinionCmd.AddMinion<EousBangboo>(choiceContext, Owner, new MinionSummonOptions(
                    MaxHp: 6m,                              // 血量
                    PrimaryStatAmount: 0m,                  // 主要参数（具体内容在随从的 OnSummon 里定义），还有次要参数等可以按需传入
                    Source: null,                           // 召唤来源（通常是这张牌）
                    Position: MinionPosition.Front));       // 站位（见后文，默认是前排）
                }
        }

        public override bool IsAllowed(IRunState runState)
        {
            return runState.Players.Count == 1;
        }

        public override bool ShouldAllowFreeTravel()
        {
            return !IsUsedUp;
        }

        public override Task AfterRoomEntered(AbstractRoom room)
        {
            if (IsUsedUp)
            {
                return Task.CompletedTask;
            }
            if (base.Owner.RunState.CurrentRoomCount > 1)
            {
                return Task.CompletedTask;
            }
            if (!(base.Owner.RunState is RunState runState))
            {
                return Task.CompletedTask;
            }
            if (runState.VisitedMapCoords.Count <= 1)
            {
                return Task.CompletedTask;
            }
            IReadOnlyList<MapCoord> visitedMapCoords = runState.VisitedMapCoords;
            MapCoord coord = visitedMapCoords[visitedMapCoords.Count - 2];
            MapPoint point = runState.Map.GetPoint(coord);
            if (point == null)
            {
                return Task.CompletedTask;
            }
            MapPoint currentMapPoint = base.Owner.RunState.CurrentMapPoint;
            if (currentMapPoint == null)
            {
                return Task.CompletedTask;
            }
            if (point.Children.Contains(currentMapPoint))
            {
                return Task.CompletedTask;
            }
            TimesUsed++;
            return Task.CompletedTask;
        }

        private void CheckIfUsedUp()
        {
            if (IsUsedUp)
            {
                //base.Status = RelicStatus.Disabled;
            }
        }
    }
}
