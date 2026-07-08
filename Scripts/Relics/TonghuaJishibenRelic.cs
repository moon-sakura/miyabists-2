using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Service;
using Steamworks;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class TonghuaJishibenRelic : ModRelicTemplate, IModRightClickableRelic
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => "res://images/relics/tonghuajishiben.png"; // TODO: 替换为专属图标 tonghuaJishiben.png
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<ChangYeShipian>(),
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new StringVar("Monster", "None"),
        ];

        // --- 计数器显示 ---
        protected bool _summoned = false;
        //public override bool ShowCounter => true;
        //public override int DisplayAmount => _summonedCount;

        //[SavedProperty]
        //public int SummonedCount
        //{
        //    get => _summonedCount;
        //    private set
        //    {
        //        AssertMutable();
        //        _summonedCount = value;
        //        InvokeDisplayAmountChanged();
        //    }
        //}

        // --- 记录的怪物类型 ---
        //private string _recordedMonsterType;

        //[SavedProperty]
        //public string RecordedMonsterType
        //{
        //    get => _recordedMonsterType;
        //    private set
        //    {
        //        AssertMutable();
        //        _recordedMonsterType = value;
        //        // 同步 DynamicVar 用于显示
        //        if (DynamicVars != null && DynamicVars.TryGetValue("RECORDED", out var dv))
        //        {
        //            dv.BaseValue = string.IsNullOrEmpty(value) ? "无" : GetMonsterDisplayName(value);
        //        }
        //    }
        //}

        protected string _recordedMonsterType = null;

        /// <summary>
        /// 已记录怪物的 Type.FullName，用于存档序列化。
        /// 召唤时通过 ModelDb.Monster&lt;T&gt;() 反射还原为 MonsterModel。
        /// </summary>
        [SavedProperty]
        public string RecordedMonsterType
        {
            get => _recordedMonsterType;
            private set
            {
                AssertMutable();
                _recordedMonsterType = value;
            }
        }

        protected string _recordedMonsterName = "";
        [SavedProperty]
        public string RecordedMonsterName
        {
            get => _recordedMonsterName;
            private set
            {
                AssertMutable();
                _recordedMonsterName = value;
            }
        }

        /// <summary>
        /// 记录怪物（由 ChangYeShipian 卡牌调用）
        /// </summary>
        public void RecordMonster(Creature target)
        {
            if (target == null || !target.IsMonster) return;

            if (target.Monster != null)
            {
                RecordedMonsterType = target.Monster.GetType().FullName;
                // 同步 DynamicVar 用于显示
                if (DynamicVars != null && DynamicVars.TryGetValue("Monster", out var dv))
                {
                    ((StringVar)dv).StringValue = target.Name;
                    RecordedMonsterName = target.Name;
                }
            }
        }

        //private static string GetMonsterDisplayName(string typeName)
        //{
        //    return typeName ?? "无";
        //}

        // --- 回合开始：第二回合开始时加入长夜诗篇 ---
        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Creature.Side) return;

            if ((Owner.Creature.CombatState.RoundNumber == 2 && Owner.Creature.CombatState.RunState.CurrentRoom.RoomType != RoomType.Elite && Owner.Creature.CombatState.RunState.CurrentRoom.RoomType != RoomType.Boss)
                || (Owner.Creature.CombatState.RoundNumber == 4 && Owner.Creature.CombatState.RunState.CurrentRoom.RoomType != RoomType.Boss && Owner.Creature.CombatState.RunState.CurrentRoom.RoomType != RoomType.Monster)
                )
            {
                Flash();
                CardModel card = Owner.Creature.CombatState.CreateCard<ChangYeShipian>(Owner.Creature.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner, CardPilePosition.Random);
            }
        }

        // --- 右键点击：将记录的怪物加入战场 ---
        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            return !string.IsNullOrEmpty(RecordedMonsterType) && !_summoned;
        }

        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            if (string.IsNullOrEmpty(RecordedMonsterType) || _summoned) return;

            Flash();

            await MiyabiFuncBase.AddMonsterAsPet(context.PlayerChoiceContext, RecordedMonsterType, Owner);

            _summoned = true;
        }

        // --- 战斗结束重置 ---
        public override Task AfterRoomEntered(AbstractRoom room)
        {
            _summoned = false;
            ((StringVar)DynamicVars["Monster"]).StringValue = RecordedMonsterName;
            return base.AfterRoomEntered(room);
        }
        //public override Task AfterCombatEnd(CombatRoom room)
        //{
        //    _summoned = false;
        //    return base.AfterCombatEnd(room);
        //}
    }
}