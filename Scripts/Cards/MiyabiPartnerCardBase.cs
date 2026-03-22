using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;


namespace Miyabists2.Scripts.Cards
{
    internal abstract class MiyabiPartnerCardBase : MiyabiCardBase
    {
        // 伙伴卡通常消耗的支援点数变量名
        protected const string SupportCostVarName = "SUPPORT_COST";
        protected int _supportCost = 0; // 默认需要 0 点支援点数

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected MiyabiPartnerCardBase(int energy, CardRarity rarity, TargetType target, bool showInLib)
            : base(energy, CardType.Skill, rarity, target, showInLib)
        {
            // 伙伴卡在视觉上可以加入特定词条
            // this.CanonicalKeywords = [MiyabiKeywords.Partner]; 
        }

        
    }
}
