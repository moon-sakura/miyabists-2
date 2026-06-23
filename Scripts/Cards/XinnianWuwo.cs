using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class XinnianWuwo : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/xinnianWuwo.png";

        public XinnianWuwo() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromCard<ShuangYue>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<XinnianWuwoPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }


        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
