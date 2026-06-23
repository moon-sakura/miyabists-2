using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards.NoneShow
{
    internal class RelicClick : ModCardTemplate
    {
        public RelicClick() : base(0, CardType.Status, CardRarity.Status, TargetType.None, false)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ShufaChoice shufa = base.Owner.Creature.CombatState?.CreateCard<ShufaChoice>(base.Owner);
            VigorChoice vigor = base.Owner.Creature.CombatState?.CreateCard<VigorChoice>(base.Owner);
            ThornsChoice thorns = base.Owner.Creature.CombatState?.CreateCard<ThornsChoice>(base.Owner);


            if (shufa != null && vigor != null && thorns != null)
            {
                List<CardModel> options = new List<CardModel> { shufa, vigor, thorns };
                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
                if (chosen is ShufaChoice)
                {
                    await ChoosePower(choiceContext, 3);
                }
                else if (chosen is VigorChoice)
                {
                    await ChoosePower(choiceContext, 1);
                }
                else if (chosen is ThornsChoice)
                {
                    await ChoosePower(choiceContext, 2);
                }
            }
        }

        public async Task ChoosePower(PlayerChoiceContext choiceContext, int choose, bool random = false)
        {
            int c = choose;
            if (random)
            {
                c = MiyabiFuncBase.RandomInt(1, 4, Owner);
            }

            switch (c)
            {
                case 1:
                    await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].BaseValue, Owner.Creature, null);
                    break;
                case 2:
                    await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars["Thorns"].BaseValue, Owner.Creature, null);
                    break;
                case 3:
                    foreach (var enemy in Owner.Creature.CombatState.HittableEnemies)
                    {
                        await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, DynamicVars["Shufa"].BaseValue, Owner.Creature, null);
                    }
                    break;
                default:
                    await PlayerCmd.GainEnergy(1, Owner);
                    break;
            }
        }
    }
}
