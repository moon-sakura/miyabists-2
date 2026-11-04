using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 决裁 - 稀有能力卡
    /// 回合开始时，随机将 1 张《遗忘》或《失感》加入手卡。
    /// 能力被移除时，变更为《明心镜·决裁》。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class JueCai : GuangCardBase
    {
        /// <summary>卡图：《明心镜·决裁》能力图标与这张卡共用同一素材</summary>
        public const string SharedArtPath = "res://images/_Guang/cards/common.png";

        public JueCai() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override string ArtPath => SharedArtPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MingxinJueCaiPower>(),
            HoverTipFactory.FromCard<ShiGan>(),
            HoverTipFactory.FromCard<YiWang>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<JueCaiPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate); // 升级后保留
        }
    }
}
