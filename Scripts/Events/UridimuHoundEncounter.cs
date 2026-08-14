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

        public override bool ShouldGiveRewards => RanOutOfTime;

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

        public UridimuHoundEncounter() : base() // 这个遭遇的房间类型，这里是普通怪物
        {
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
