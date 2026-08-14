using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts.Enemies;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;

namespace Miyabists2.Scripts.Events
{
    [RegisterGlobalEncounter]
    internal class UridimuHoundEncounter : ModEncounterTemplate
    {
        // 所有可能出现的怪物
        public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<OldHuntingDogEnemy>()];

        // 这个遭遇在哪些层级出现
        public override bool IsValidForAct(ActModel act) => false; // 事件可在任意幕触发

        public override RoomType RoomType => RoomType.Elite;

        public override bool ShouldGiveRewards => !RanOutOfTime;

        private bool _ranOutOfTime = false;

        public bool RanOutOfTime
        {
            get
            {
                return _ranOutOfTime;
            }
            set
            {
                AssertMutable();
                _ranOutOfTime = value;
            }
        }

        // 本次战斗是第几次遇到猎犬（由迷宫诡域事件在进入战斗前设置，默认1=无强化）。
        // 注意：遭遇是全局共享的 canonical 模型，进入战斗时框架会自行 ToMutable() 生成副本，
        // 因此战斗内临时数据不能写进 canonical 实例，只能通过静态字段传给 GenerateMonsters()。
        public static int NextEncounterIndex = 1;

        // 事件侧的累计战斗概率（由迷宫诡域事件在进入战斗前写入，SL 后从这里恢复）。
        // 因为遭遇房间的 EncounterState 是唯一会被框架写入存档的字符串字典，
        // 事件自身的字段（包括 _houndEncounters）不随 SL 保留，只能借战斗房间带出来。
        public static int EventBaseCombatChance = 20;
        public static int EventCombatChance = 20;

        public UridimuHoundEncounter() : base() // 这个遭遇的房间类型，这里是普通怪物
        {
        }

        // 持久化：保存（pre-finished）战斗房间时，框架会调用 Encounter.SaveCustomState()
        // 把返回值写进 SerializableRoom.EncounterState；读档恢复战斗时再调用 LoadCustomState 还原。
        // 这样 SL / 退出重进时，事件能在 Resume 里读回遭遇次数与战斗概率。
        public override Dictionary<string, string> SaveCustomState()
        {
            return new Dictionary<string, string>
            {
                ["RanOutOfTime"] = RanOutOfTime.ToString(),
                ["NextEncounterIndex"] = NextEncounterIndex.ToString(),
                ["EventBaseCombatChance"] = EventBaseCombatChance.ToString(),
                ["EventCombatChance"] = EventCombatChance.ToString(),
            };
        }

        public override void LoadCustomState(Dictionary<string, string> state)
        {
            RanOutOfTime = bool.Parse(state["RanOutOfTime"]);
            if (state.TryGetValue("NextEncounterIndex", out string? idx)) NextEncounterIndex = int.Parse(idx);
            if (state.TryGetValue("EventBaseCombatChance", out string? bcc)) EventBaseCombatChance = int.Parse(bcc);
            if (state.TryGetValue("EventCombatChance", out string? cc)) EventCombatChance = int.Parse(cc);
        }

        private MonsterModel GetEnemy()
        {
            var en = ModelDb.Monster<OldHuntingDogEnemy>().ToMutable();
            ((OldHuntingDogEnemy)en).NextEncounterIndex = NextEncounterIndex;
            return en;
        }

        // 不要忘了这里的model需要调用ToMutable()，表示不是标准值而是战斗中的可变数据
        protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
            (GetEnemy(), null) // 如果不想指定怪物生成在哪个槽位，可以直接传null，系统会自动分配
        ];
    }
}
