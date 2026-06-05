using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Enemies;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class EousBangbooRelic : MiyabiBangbooRelicBase
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override string PackedIconPath => "res://images/bangboo/relicMode/eousRelic.png";
        private const int _roomCount = 2;
        private int _timesUsed;

        public override bool IsUsedUp => TimesUsed >= 2;
        public override bool ShowCounter => !IsUsedUp;
        public override int DisplayAmount => 2 - TimesUsed;


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
                base.DynamicVars["Rooms"].BaseValue = 2 - _timesUsed;
                InvokeDisplayAmountChanged();
                CheckIfUsedUp();
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Rooms", 2m)];


        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber != 1) return;

            Flash();


            await MiyabiBangbooService.SummonBangboo<EousBangboo>(Owner, 6m, MinionPosition.Back,null,1m);

            //var bangboo = await PlayerCmd.AddPet<EousBangboo>(Owner);

            //NCreature node = NCombatRoom.Instance?.GetCreatureNode(bangboo);
            //if (node != null)
            //{
            //    node.Modulate = Colors.Transparent;
            //    Tween tween = node.CreateTween();
            //    tween.TweenProperty(node, "modulate", Colors.White, 0.3499999940395355).SetDelay(0.10000000149011612);
            //    //node.StartReviveAnim();
            //}
            ////await PowerCmd.Apply<DieForYouPower>(choiceContext, bangboo, 1m, null, null);
            //node?.TrackBlockStatus(Owner.Creature);
            //node?.ToggleIsInteractable(on:true);
            //await CreatureCmd.SetMaxHp(bangboo, 6);
            //await CreatureCmd.Heal(bangboo, 6);
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

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<EousAct>(),
        ];
    }
}
