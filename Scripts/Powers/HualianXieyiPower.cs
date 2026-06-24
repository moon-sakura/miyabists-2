using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Ui.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class HualianXieyiPower : ModPowerTemplate, IModRightClickablePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/hualianXieyi.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(0,ValueProp.Unpowered),
            new EnergyVar(1)
        ];

        public void SetDamage(int damage)
        {
            DynamicVars.Damage.BaseValue += damage;
        }

        // 可选：本地预检，返回 false 则本次右键不会触发
        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            SupportPointPower p = base.Owner.GetPower<SupportPointPower>();
            if(p != null)   
                return p.CanUsePoint(2) > 0;

            return false;
        }

        // 右键执行（多人下会在所有客户端同步执行）
        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            List<Creature> enemies = base.CombatState.Enemies
                    .Where((Creature e) => e != null && e.IsAlive)
                    .ToList();

            if (enemies.Count > 0)
            {
                NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(Owner, enemies.Last());
                if (nHyperbeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }

                foreach (Creature item in enemies)
                {
                    NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(Owner, item);
                    if (nHyperbeamImpactVfx != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                    }
                }

                await CreatureCmd.Damage(context.PlayerChoiceContext, enemies, DynamicVars.Damage, Owner);

                await PlayerCmd.GainEnergy(Amount, Owner.Player);

                await PowerCmd.Apply<SupportPointPower>(context.PlayerChoiceContext, base.Owner, -2, null, null);
            }
        }
    }
}
