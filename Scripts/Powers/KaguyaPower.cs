using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class KaguyaPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomPackedIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;


        public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            for(int i = 0; i < base.Amount; i++)
            {
                await DoRandomEffect(choiceContext);
            }
            Flash();
        }

        private async Task DoRandomEffect(PlayerChoiceContext choiceContext)
        {
            int result = base.Owner.Player.RunState.Rng.Shuffle.NextInt(1, 13);
            if (result == 1)
            {
                await CreatureCmd.Heal(base.Owner, 3m);
            }
            else if (result == 2)
            {
                await PowerCmd.Apply<SlipperyPower>(base.Owner, 1m, base.Owner, null);
            }
            else if (result == 3)
            {
                await PowerCmd.Apply<PlatingPower>(base.Owner, 3m, base.Owner, null);
            }
            else if (result == 4)
            {
                await PowerCmd.Apply<ArtifactPower>(base.Owner, 1m, base.Owner, null);
            }
            else if (result == 5)
            {
                await PowerCmd.Apply<StrengthPower>(base.Owner, 1m, base.Owner, null);
            }
            else if (result == 6)
            {
                await CreatureCmd.GainBlock(base.Owner, 4, ValueProp.Unpowered, null);
            }
            else if (result == 7)
            {
                await PlayerCmd.GainEnergy(1, base.Owner.Player);
            }
            else if (result == 8)
            {
                foreach (Creature Enemy in base.CombatState.HittableEnemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        await CreatureCmd.Damage(choiceContext, Enemy, 4m, ValueProp.Unpowered, base.Owner, null);
                    }
                }
            }
            else if (result == 9)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        //await PowerCmd.Apply<AnomalyBuildupPower>(Enemy, 1m, null, null);
                        await MiyabiCombatService.AddAnoBuildup(Enemy, 1, base.Owner, null, choiceContext);
                    }
                }
            }
            else if (result == 10)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        //await PowerCmd.Apply<DazePower>(Enemy, 10m, null, null);
                        await MiyabiCombatService.AddDaze(Enemy, new DynamicVar("Daze",10), base.Owner);
                    }
                }
            }
            else if (result == 11)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        await PowerCmd.Apply<WeakPower>(Enemy, 1m, null, null);
                    }
                }
            }
            else if (result == 12)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive)
                    {
                        await PowerCmd.Apply<VulnerablePower>(Enemy, 1m, null, null);
                    }
                }
            }
        }
    }
}
