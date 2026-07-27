using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 守华照寂 - 1费Rare能力卡
    /// 获得格挡时变为0格挡，然后获得1点能量，抽1张卡
    /// 升级后变为0费
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ShouhuaZhaoji : YixuanCardBase
    {
        public ShouhuaZhaoji() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        // override string ArtPath => "res://images/_YiXuan/cards/shouhuaZhaoji.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(1),
            new DynamicVar("DrawCount", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShouhuaZhaojiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // TODO: Apply ShouhuaZhaojiPower - when gaining Block, set Block to 0, gain Energy, draw card
            await PowerCmd.Apply<ShouhuaZhaojiPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 1费 → 0费
        }
    }
}
