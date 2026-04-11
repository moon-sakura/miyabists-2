using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class QuickSupport : MiyabiBlockCardBase
    {

        public QuickSupport() : base(1, CardRarity.Uncommon, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4,ValueProp.Unpowered),
            new DynamicVar(ParryVarName, 1)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal
        ];

        public override bool GainsBlock => false;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, DynamicVars.Damage, this);

            CardPile discardPile = PileType.Draw.GetPile(base.Owner);

            IEnumerable<CardModel> selectedCards = discardPile.Cards
                .Where(c => c.CanonicalKeywords.Contains(MiyabiKeywords.Friends))
                .TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection);

            if (selectedCards.Count() != 0)
            {
                foreach (CardModel item in selectedCards)
                {
                    var target = base.Owner.Creature;
                    if (item.TargetType == TargetType.AnyEnemy)
                    {
                        target = base.Owner.Creature.CombatState.HittableEnemies.ToList().MaxBy(e => e.CurrentHp);
                    }

                    await CardCmd.AutoPlay(choiceContext, item, target);
                }
            }
        }


        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(-2);
        }
    }
}
