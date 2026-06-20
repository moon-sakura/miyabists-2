using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts._Yixuan.Powers.CinimaPower;
using Miyabists2.Scripts.Patches;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics.SpecRelic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Relics.Shared
{
    [RegisterRelic(typeof(YixuanRelicPool))]
    internal class ChoukaRelicYixuan : ChoukaRelic, ISharedType
    {
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }

            DynamicVars["CINIMA"].BaseValue = CinimaCounter;

            if (base.Owner.Creature.CombatState.RoundNumber == 1 && Owner.Character is Yixuan)
            {
                Flash();
                if (CinimaCounter >= 1)
                {
                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<FufaQianchong>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                }

                if (CinimaCounter >= 2)
                {
                    await PowerCmd.Apply<YixuanCinimatwoPower>(choiceContext, base.Owner.Creature, 1m, null, null);
                }

                if (CinimaCounter >= 3)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 4m, null, null);
                }

                if (CinimaCounter >= 4)
                {
                    foreach (var enemy in Owner.Creature.CombatState.HittableEnemies)
                    {
                        await PowerCmd.Apply<FumoPower>(choiceContext, enemy, 1m, null, null);
                    }
                }

                if (CinimaCounter >= 5)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 6m, null, null);
                }

                if (CinimaCounter >= 6)
                {
                    await PowerCmd.Apply<YixuanCinimasixPower>(choiceContext, base.Owner.Creature, 2m, null, null);
                }
            }
        }
    }
}
