using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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

            // 消耗1张手卡
            CardModel target = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext,
                player: Owner,
                filter: null,
                source: this)).FirstOrDefault();

            if (target != null)
            {
                await CardCmd.Exhaust(choiceContext, target);
            }

            // 本回合下一张攻击卡额外打出1次（连环拳）
            await PowerCmd.Apply<OneTwoPunchPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain); // 升级后保留
        }

        // 被消耗时获得势不可挡
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            // 势不可挡（JuggernautPower）：每次获得格挡时对随机敌人造成伤害
            await PowerCmd.Apply<JuggernautPower>(choiceContext, Owner.Creature,
                DynamicVars["UnstoppableOnExhaust"].IntValue, Owner.Creature, this);
        }
    }
}
