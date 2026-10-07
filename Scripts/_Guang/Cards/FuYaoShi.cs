using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 扶摇势 - 普通技能卡
    /// 获得3点活力，获得1点临时力量。附加：被消耗时获得8点活力。
    /// 升级：4点活力，2点临时力量。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class FuYaoShi : GuangCardBase
    {
        public FuYaoShi() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/fuYaoShi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Vigor", 3),
            new DynamicVar("TempStrength", 1),
            new DynamicVar("VigorOnExhaust", 8),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<StrengthPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature,
                DynamicVars["Vigor"].IntValue, Owner.Creature, this);

            // ========== 待实现 ==========
            // TODO: 获得 DynamicVars["TempStrength"] 点临时力量
            // 临时力量需要"回合结束时消失"的力量能力（本项目暂未确认对应能力），待补充。
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Vigor"].UpgradeValueBy(1);
            DynamicVars["TempStrength"].UpgradeValueBy(1);
        }

        // ========== 待实现 ==========
        // TODO: 被消耗时获得 DynamicVars["VigorOnExhaust"] 点活力
        // 需要挂接本卡被消耗的时机回调。
    }
}
