using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    public abstract class GuangCardBase : MiyabiCardBase
    {
        protected GuangCardBase(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        protected override string ArtPath => "res://images/_Guang/cards/common.png";


        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar))
            {
                await MiyabiCombatService.AddDaze(choiceContext, target, dazeVar, base.Owner.Creature);
            }
        }

        protected virtual async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
        }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
        {
            if(card == this)
            {
                await ExhaustEffect(choiceContext);
            }
        }

        // ==================== 青溟剑势消耗 ====================
        // 与支援点数(SupportPointPower)的 SupportPointFunc / CheckSupportCost / CostSupporPoint 一一对应

        /// <summary>青溟剑势变量名，用于 CanonicalVars 里的 new DynamicVar(JianshiVarName, n)</summary>
        protected const string JianshiVarName = "QINGMING_JIANSHI_POWER";

        /// <summary>
        /// 花费青溟剑势执行一段效果：点数够（或有免费次数）才执行 effect，执行后扣除。
        /// 用法：await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () => { ... });
        /// </summary>
        protected async Task JianshiFunc(PlayerChoiceContext choiceContext, int cost, Func<Task> effect, bool isForceTrigger = false)
        {
            if (CheckJianshiCost(cost) != 0 || isForceTrigger)
            {
                await effect();
                await CostJianshi(cost, choiceContext);
            }
        }

        /// <summary>0 = 点不起，1 = 正常扣除，2 = 免费（SetFree 后有效）</summary>
        public virtual int CheckJianshiCost(int a)
        {
            if (!Owner.Creature.HasPower<QingmingJianshiPower>()) return 0;
            return Owner.Creature.GetPower<QingmingJianshiPower>().CanUsePoint(a);
        }

        public virtual async Task CostJianshi(int amount, PlayerChoiceContext choiceContext)
        {
            if (CheckJianshiCost(amount) == 0) return;
            if (CheckJianshiCost(amount) == 1)
                await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature, -amount, Owner.Creature, this);
            if (CheckJianshiCost(amount) == 2) return;
        }
    }
}
