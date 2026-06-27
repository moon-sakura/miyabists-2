using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Cards.NoneShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class JieFa : YixuanCardBase
    {
        public JieFa() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/jiefa.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Percent",100),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            GD.Print(choiceContext.GetType().FullName);

            Vigor2thorns vigor = base.Owner.Creature.CombatState?.CreateCard<Vigor2thorns>(base.Owner);
            Thorns2vigor thorns = base.Owner.Creature.CombatState?.CreateCard<Thorns2vigor>(base.Owner);


            if (vigor != null && thorns != null)
            {
                List<CardModel> options = new List<CardModel> { vigor, thorns };

                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
                if (chosen is Vigor2thorns)
                {
                    int amount = Owner.Creature.GetPowerAmount<VigorPower>();
                    await PowerCmd.Remove(Owner.Creature.GetPower<VigorPower>());
                    await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, amount * DynamicVars["Percent"].BaseValue / 100, Owner.Creature, this);
                }
                else if (chosen is Thorns2vigor)
                {
                    int amount = Owner.Creature.GetPowerAmount<ThornsPower>();
                    await PowerCmd.Remove(Owner.Creature.GetPower<ThornsPower>());
                    await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, amount * DynamicVars["Percent"].BaseValue / 100, Owner.Creature, this);
                }
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Percent"].BaseValue += 50;
            base.OnUpgrade();
        }
    }
}
