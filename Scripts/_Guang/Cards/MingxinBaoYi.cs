using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心境·抱一（原卡：止戈）
    /// 获得9点格挡，获得1层倒映（reflect）。
    /// 升级：14点格挡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinBaoYi : GuangMingxinjingCardBase
    {
        public MingxinBaoYi() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(9, ValueProp.Move),
            new DynamicVar("Reflect", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            // TODO: 倒映(reflect)能力写好后取消注释
            // HoverTipFactory.FromPower<ReflectPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // ========== 待实现 ==========
            // TODO: 获得 DynamicVars["Reflect"] 层倒映（reflect）
            // 倒映能力本项目暂未确认对应实现，待补充。
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(5);
        }
    }
}
