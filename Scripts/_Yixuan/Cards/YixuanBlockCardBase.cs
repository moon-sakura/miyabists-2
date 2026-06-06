using MegaCrit.Sts2.Core.Entities.Cards;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    public abstract class YixuanBlockCardBase : MiyabiCardBase
    {
        public YixuanBlockCardBase(int baseCost, CardRarity rarity, TargetType target, CardType type = CardType.Skill, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";
    }
}
