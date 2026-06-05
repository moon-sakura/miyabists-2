using STS2RitsuLib.Interop.AutoRegistration;
using Miyabists2.Scripts.Service;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Rooms;

namespace Miyabists2.Scripts.Events
{
    internal class MiyabiTestEncounter : ModEncounterTemplate
    {
        // 所有可能出现的怪物
        public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<MiyabiGhostEnemy>()];

        // 这个遭遇在那些层级出现
        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 2; // 只在第二幕出现

        // 这个遭遇是否是弱怪池
        //public override bool IsWeak => false;

        public override RoomType RoomType => RoomType.Elite;

        public MiyabiTestEncounter() : base() // 这个遭遇的房间类型，这里是普通怪物
        {
        }

        // 不要忘了这里的model需要调用ToMutable()，表示不是标准值而是战斗中的可变数据
        protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
            (ModelDb.Monster<MiyabiGhostEnemy>().ToMutable(), null) // 如果不想指定怪物生成在哪个槽位，可以直接传null，系统会自动分配
        ];
    }
}
