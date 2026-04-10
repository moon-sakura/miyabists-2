using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class EJiZhan : MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/eJizhan.png";

        public EJiZhan() : base(3, CardRarity.Rare, TargetType.AnyEnemy, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.LieShuang,
            MiyabiKeywords.EndSkill
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(30, ValueProp.Move),
            new DynamicVar(DazeVarName, 10)
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
            await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature, 3, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(10);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(5);
        }
    }
}
