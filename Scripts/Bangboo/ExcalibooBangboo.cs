using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Helpers;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class ExcalibooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/excaliboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options);

            //if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
            await PowerCmd.Apply<ExcalibooAct>(new ThrowingPlayerChoiceContext(), self, 1m, owner.Creature, options.Source);
        }
    }

    internal class ExcalibooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AllEnemies;
        public override string BigIconPath => "res://images/bangboo/relicMode/excalibooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
            new DynamicVar("Charged",0),
            new DamageVar(40m, ValueProp.Move),
        ];

        public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;
            await ActEffect(choiceContext, null);
        }

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            DynamicVars["Charged"].BaseValue += MAXUSE;

            if (DynamicVars["Charged"].BaseValue >= 3 || MiyabiFuncBase.GetIsTrue100(20, Owner.PetOwner))
            {
                var bangboo = base.Owner;

                List<Creature> enemies = base.CombatState.Enemies
                    .Where((Creature e) => e != null && e.IsAlive)
                    .ToList();

                if (enemies.Count > 0)
                {
                    NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(bangboo, enemies.Last());
                    if (nHyperbeamVfx != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                        await Cmd.Wait(0.5f);
                    }

                    foreach (Creature item in enemies)
                    {
                        NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(bangboo, item);
                        if (nHyperbeamImpactVfx != null)
                        {
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                        }
                    }

                    await CreatureCmd.Damage(choiceContext, enemies, DynamicVars.Damage, Owner);
                    DynamicVars["Charged"].BaseValue = 0;
                }
            }
        }

        public override async Task OnCardActivate(PlayerChoiceContext choiceContext)
        {
            await ActEffect(choiceContext, null);
        }
    }
}
