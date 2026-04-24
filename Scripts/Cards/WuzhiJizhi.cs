using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class WuzhiJizhi : MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/commonCard.png";

        public WuzhiJizhi() : base(0, CardRarity.Token, TargetType.AnyEnemy, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.LieShuang,
            CardKeyword.Retain,
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(35, ValueProp.Move),
            new DynamicVar(DazeVarName, 40)
        ];


        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<FrostFallPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<FrostPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            await PowerCmd.Apply<FrostFallPower>(choiceContext, base.Owner.Creature, 2, base.Owner.Creature, this);
            await PowerCmd.Apply<FrailPower>(choiceContext, base.Owner.Creature, 99, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(15);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(10);
        }
    }
}
