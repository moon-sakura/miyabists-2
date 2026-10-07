using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    /// 斩邪祟 - 稀有技能卡
    /// 获得1点青溟剑势，消耗1张手卡，本回合下一张攻击卡额外打出1次。
    /// 附加：被消耗时获得1层势不可挡。
    /// 升级：保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhanXieSui : GuangCardBase
    {
        public ZhanXieSui() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhanXieSui.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiGain", 1),
            new DynamicVar("UnstoppableOnExhaust", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                DynamicVars["JianshiGain"].IntValue, Owner.Creature, this);

            // ========== 待实现 ==========
            // TODO: 消耗1张手卡（需要手牌选择界面）
            // await CardSelectCmd.FromChooseACardScreen(...) -> CardCmd.Exhaust(...)

            // ========== 待实现 ==========
            // TODO: 本回合下一张攻击卡额外打出1次
            // 需要一个"下张攻击卡额外打出一次"的计数能力承载，待补充。
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }

        // ========== 待实现 ==========
        // TODO: 被消耗时获得 DynamicVars["UnstoppableOnExhaust"] 层势不可挡
        // 势不可挡能力本项目暂未确认对应实现，需要挂接本卡被消耗的时机回调。
    }
}
