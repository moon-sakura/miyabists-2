using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 芳菲之邀 - 1费Ancient能力卡
    /// 每回合开始时对所有敌人造成6点伤害，并施加1层流明效果
    /// 升级后添加固有
    /// </summary>
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class FangfeiZhiyao : MiyabiCardBase
    {
        public FangfeiZhiyao() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
        ];

        protected override string ArtPath => "res://images/cards/fangfeiZhiyao.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6,ValueProp.Unpowered),
            new DynamicVar("LiumingAmount", 1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<LiumingPower>(),
            HoverTipFactory.FromPower<YaobianPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
        ];

        private static readonly string[] DanVoices = { "dan_endskill_gaoguangshike", "dan_endskill_xhongxianrongguang", "dan_endskill_ximiezhangsheng" };

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            MiyabiAudioPlay.Random(DanVoices);
            await PowerCmd.Apply<FangfeiZhiyaoPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
