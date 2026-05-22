using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Action;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class MiyabiBangbooBase : MinionModel
    {
        public override int MinInitialHp => 8; // 作为敌方方怪物生成时的血量，通常无需在意
        public override int MaxInitialHp => 8; // 作为敌方方怪物生成时的血量，通常无需在意
        protected override string VisualsPath => null; // 随从的视觉资源路径，tscn 格式，建议参考原版游戏的怪物

        public virtual MinionPosition Position { get; set; } = MinionPosition.Front;



        public MiyabiBangbooBase()
        {
            VisualsPath?.RegisterSceneForConversion<NCreatureVisuals>();
        }

        // 召唤时执行的代码，通常用来设置血量、应用初始能力等，options 是在召唤随从时传入的参数
        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options)
        {
            if (options.MaxHp is decimal maxHp)
            {
                await CreatureCmd.SetMaxAndCurrentHp(self, maxHp); // 设置血量
            }
        }
    }

    internal class MiyabiBangbooActBase : ActionModel
    {
        public override TargetType TargetType => TargetType.AnyPlayer;           // 目标类型
        public override bool AutoRemoveAtTurnEnd => false;                       // 是否在回合结束自动移除
        public override PowerType Type => PowerType.Buff;                       // Power 的类型
        public override PowerStackType StackType => PowerStackType.Counter;     // Power 的堆叠属性


        // 核心重载，定义 Action 被触发时的行为，类似于卡牌的 OnPlay
        // 和卡牌一样，如果目标无需选定（如所有敌人），target 将会是 null
        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            
        }
    }
}
