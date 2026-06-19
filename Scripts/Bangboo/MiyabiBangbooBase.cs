using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Action;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.RitsuAdapters;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class MiyabiBangbooBase : ModMinionTemplate
    {
        public override int MinInitialHp => 8; // 作为敌方方怪物生成时的血量，通常无需在意
        public override int MaxInitialHp => 8; // 作为敌方方怪物生成时的血量，通常无需在意
        protected override string VisualsPath => null; // 随从的视觉资源路径，tscn 格式，建议参考原版游戏的怪物

        //public virtual MinionPosition Position { get; set; } = MinionPosition.Front;

        public MiyabiBangbooBase()
        {
            VisualsPath?.RegisterSceneForConversion<NCreatureVisuals>();
        }

        // 召唤时执行的代码，通常用来设置血量、应用初始能力等，options 是在召唤随从时传入的参数
        public override async Task OnSummon(PlayerChoiceContext choiceContext ,Player owner, MinionSummonOptions options)
        {
            if (options.MaxHp is decimal maxHp)
            {
                await CreatureCmd.SetMaxAndCurrentHp(this.Creature, maxHp); // 设置血量
            }
        }
    }

    internal class MiyabiBangbooActBase : ModActionTemplate
    {
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override bool AutoRemoveAtTurnEnd => false;
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public virtual string BigIconPath => "res://images/bangboo/relicMode/eousRelic.png";
        //public string BigBetaIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
        ];

        public int UsedCount { get; set; } = 0;
        public int MAXUSE { get; set; } = 1;
        public int isFree { get; set; } = 0;

        public void AddFree() => isFree++;

        protected virtual bool CanPayCost => Owner.PetOwner.PlayerCombatState.Energy >= 1;

        /// <summary>Bangboo 的具体效果，子类重写。choiceContext 和 target 由调用方传入。</summary>
        public virtual async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
        }

        /// <summary>默认消耗 1 能量，子类可重写为消耗金币/血量等。</summary>
        public virtual async Task ActCost()
        {
            await PlayerCmd.LoseEnergy(1, Owner.PetOwner);
            UsedCount++;
            DynamicVars["Used"].BaseValue = UsedCount;
        }

        /// <summary>被卡牌激活时的行为。默认给一次免费使用，自动触发的 Bangboo 重写为直接执行 ActEffect。</summary>
        public virtual async Task OnCardActivate(PlayerChoiceContext choiceContext)
        {
            AddFree();
        }

        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            DynamicVars["MAXUSE"].BaseValue = MAXUSE;
            DynamicVars["Used"].BaseValue = UsedCount;

            if ((!CanPayCost || UsedCount >= MAXUSE) && isFree < 1) return;

            await ActEffect(choiceContext, target);
            if (isFree < 1)
                await ActCost();
            isFree--;
            if (isFree < 0) isFree = 0;
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            UsedCount = 0;
            DynamicVars["Used"].BaseValue = UsedCount;
            return base.AfterPlayerTurnStart(choiceContext, player);
        }

        public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            DynamicVars["MAXUSE"].BaseValue = MAXUSE;
            DynamicVars["Used"].BaseValue = UsedCount;
            return base.AfterCardPlayed(choiceContext, cardPlay);
        }
    }
}
