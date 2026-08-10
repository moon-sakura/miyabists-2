using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 无量反击 - 2费Uncommon技能卡
    /// 获得14点格挡，结束自己的回合。本回合每完全格挡一次攻击，下回合获得1点能量
    /// </summary>
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class WuliangFanji : MiyabiCardBase
    {
        public WuliangFanji() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/wuliangFanji.png";

        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(14, ValueProp.Move),
            new EnergyVar(1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.OtherWorldFriends,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<WuliangFanjiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 获得14点格挡
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            // 应用反击计数能力：本回合完全格挡攻击 → 下回合获得能量
            await PowerCmd.Apply<WuliangFanjiPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // TODO: 结束自己的回合
            PlayerCmd.EndTurn(Owner, false);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(4); // 14 → 18
        }
    }
}
