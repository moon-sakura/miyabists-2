using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MinionLib.Commands;
using MinionLib.Layout;
using MinionLib.Powers;
using MinionLib.Targeting;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class BangbooHelpmeOne : MiyabiCardBase
    {
        //protected override string ArtPath => $"res://images/cards/zhaojiaZhunbei.png";

        public BangbooHelpmeOne() : base(1, CardType.Skill, CardRarity.Common, MinionTargetTypes.AnyMinion) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
            HoverTipFactory.FromPower<MiyabiGuardianPower>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            //CardKeyword.Exhaust
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target is not { Monster: MiyabiBangbooBase } target) return;

            

            await PowerCmd.Apply<MiyabiGuardianPower>(choiceContext, target, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            //RemoveKeyword(CardKeyword.Exhaust);
            AddKeyword(CardKeyword.Retain);
            base.OnUpgrade();
        }
    }
}
