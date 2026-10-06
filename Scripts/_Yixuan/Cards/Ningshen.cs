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

namespace Miyabists2.Scripts._Yixuan.Cards
{
    /// <summary>
    /// 凝神 - 2费Rare能力卡
    /// 格挡不再在你的回合开始时消失，每回合结束时受到8点伤害
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class Ningshen : YixuanCardBase
    {
        public Ningshen() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/ningshen.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DisintegrationAmount", 8),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BarricadePower>(),
            HoverTipFactory.FromPower<DisintegrationPower>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 格挡不再在回合开始时消失（永续能力，1层即可）
            await PowerCmd.Apply<BarricadePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // 每回合结束时受到8点伤害（层数即每回合受到的伤害）
            await PowerCmd.Apply<DisintegrationPower>(choiceContext, Owner.Creature, DynamicVars["DisintegrationAmount"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // TODO: 升级效果未指定
            RemoveKeyword(CardKeyword.Ethereal); // 升级后移除虚无
        }
    }
}
