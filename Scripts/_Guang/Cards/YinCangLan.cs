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
    /// 引沧澜 - 初始攻击卡
    /// 对敌人造成5点伤害。将1张随机普通牌加入你的手牌。
    /// 升级：加入的普通牌添加保留。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class YinCangLan : GuangCardBase
    {
        public YinCangLan() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/yinCangLan.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Move),
            new CardsVar(1),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // ========== 待实现 ==========
            // TODO: 随机取1张普通牌(Common)加入手牌
            // 升级后加入的卡需要 AddKeyword(CardKeyword.Retain)
        }

        protected override void OnUpgrade()
        {
            // TODO: 升级后加入的随机普通牌添加保留（需要在生成时打标记）
        }
    }
}
