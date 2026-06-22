using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class HuaFa : MiyabiBlockCardBase
    {
        protected override string ArtPath => $"res://images/cards/huaFa.png";

        public HuaFa() : base(2, CardRarity.Uncommon,true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(16, ValueProp.Move),
            new DynamicVar(ParryVarName, 2),
            new DynamicVar(SlipperyVarName, 0)
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int amount = CombatManager.Instance.History.CardPlaysFinished
                .Count((CardPlayFinishedEntry e) 
                => e.CardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends) 
                && e.CardPlay.Card.Owner == base.Owner 
                && e.HappenedThisTurn(base.CombatState));
            await base.OnPlay(choiceContext, cardPlay);
            if (amount > 0)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, 6m, ValueProp.Unpowered, cardPlay);
                await PowerCmd.Apply<MiyabiParryPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
            }

        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(4);

            if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
