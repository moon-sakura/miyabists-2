using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 武者修养 - 2费Uncommon能力卡
    /// 每次使用花辞后抽1张卡
    /// 升级后变为1费
    /// </summary>
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class WuzheXiuyang : MiyabiCardBase
    {
        public WuzheXiuyang() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/cards/wuzheXiuyang.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DrawCount", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<WuzheXiuyangPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<WuzheXiuyangPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 2费 → 1费
        }
    }
}
