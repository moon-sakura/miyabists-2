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
    /// 梦中身 - 罕见技能卡
    /// 获得 1 层无实体。
    /// 被消耗时，抽 2 张卡，获得 2 点能量。
    /// 升级：2 费。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MengZhongShen : GuangCardBase
    {
        public MengZhongShen() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("IntangibleGain", 1),
            new CardsVar(2),
            new EnergyVar(2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<IntangiblePower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature,
                DynamicVars["IntangibleGain"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后 2 费
        }

        // 被消耗时：抽 2 张卡，获得 2 点能量
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }
}
