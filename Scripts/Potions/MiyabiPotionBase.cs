using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Char;

namespace Miyabists2.Scripts.Potions
{
    [RegisterPotion(typeof(MiyabiPotionPool))]
    internal abstract class MiyabiPotionBase : ModPotionTemplate
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            
        ];

        // 药水图片。不一定svg，只要最终能变成Texture的格式就行。
        public override string? CustomImagePath => "res://images/potions/commonPotions.png";
        public override string? CustomOutlinePath => CustomImagePath;

    }
}
