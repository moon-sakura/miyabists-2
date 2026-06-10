using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    public abstract class YixuanCardBase : MiyabiCardBase
    {
        protected YixuanCardBase(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        protected override string ArtPath => base.ArtPath;

        protected const string VigorVarName = "VIGOR_POWER";
        protected const string ThornsVarName = "THORNS_POWER";
        protected const string ShufaVarName = "SHUFA_POWER";
        protected const string ShannengVarName = "SHANNENG_POWER";

        /// <summary> 检查闪能是否足够 </summary>
        /// <returns>0: 不可用, 1: 可用 </returns>
        protected virtual int CheckShannengCost(int amount)
        {
            if (!Owner.Creature.HasPower<ShannengPower>()) return 0;
            return Owner.Creature.GetPower<ShannengPower>().CanUseShanneng(amount) ? 1 : 0;
        }

        /// <summary> 消耗闪能 </summary>
        protected virtual async Task CostShanneng(int amount, PlayerChoiceContext choiceContext)
        {
            if (CheckShannengCost(amount) == 0) return;
            await Owner.Creature.GetPower<ShannengPower>().UseShanneng(choiceContext, amount);
        }

        /// <summary> 闪能条件触发：检查闪能 → 执行动作 → 消耗闪能 </summary>
        protected async Task ShannengFunc(PlayerChoiceContext choiceContext, int cost, Func<Task> action, bool isForceTrigger = false, bool isFreeCost = false)
        {
            if (CheckShannengCost(cost) != 0 || isForceTrigger)
            {
                await action();
                if (!isFreeCost)
                    await CostShanneng(cost, choiceContext);
            }
        }
    }
}
