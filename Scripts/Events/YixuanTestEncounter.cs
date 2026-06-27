using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts.Enemies;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Events
{
    [RegisterActEncounter(typeof(Glory))]
    internal class YixuanTestEncounter : ModEncounterTemplate
    {
        // 所有可能出现的怪物
        public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<YixuanEnemy>()];

        // 这个遭遇在那些层级出现
        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 3 && MiyabiModConfig.MiyabiEnemiesAppear; // 只在第三幕出现

        // 这个遭遇是否是弱怪池
        //public override bool IsWeak => false;

        public override RoomType RoomType => RoomType.Elite;

        public YixuanTestEncounter() : base() // 这个遭遇的房间类型，这里是普通怪物
        {
        }

        // 不要忘了这里的model需要调用ToMutable()，表示不是标准值而是战斗中的可变数据
        protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
            (ModelDb.Monster<YixuanEnemy>().ToMutable(), null) // 如果不想指定怪物生成在哪个槽位，可以直接传null，系统会自动分配
        ];
    }
}
