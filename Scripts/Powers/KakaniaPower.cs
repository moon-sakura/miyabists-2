using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interactions.RightClick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    internal class KakaniaPower : ModPowerTemplate, IModRightClickablePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/kakania.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("RecordedDamage", 0),
            new DynamicVar("OutgoingDamage", 0),
            new DynamicVar("BurstRatio", 30),
        ];

        private bool isRecord = false;

        public void SetBurstRatio(int ratio)
        {
            DynamicVars["BurstRatio"].BaseValue = ratio;
        }

        /// <summary>增加受到的伤害 25%</summary>
        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        {
            if (target == Owner && !props.HasFlag(ValueProp.Unpowered))
                return 1.25m;
            return 1m;
        }

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner)
                return;

            // 记录受到的所有伤害
            DynamicVars["RecordedDamage"].BaseValue += (int)result.TotalDamage * Amount;
        }

        /// <summary>触发后记录造成的伤害</summary>
        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (dealer != Owner) return;
            if (isRecord && !props.HasFlag(ValueProp.Unpowered))
            {
                DynamicVars["OutgoingDamage"].BaseValue += (int)result.TotalDamage * Amount;
            }
        }

        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            SupportPointPower p = base.Owner.GetPower<SupportPointPower>();
            if (p != null && p.CanUsePoint(5) > 0)
                return true;
            return false;
        }

        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            int recorded = DynamicVars["RecordedDamage"].IntValue;
            //if (recorded <= 0) return;

            List<Creature> enemies = base.CombatState.Enemies
                .Where(e => e != null && e.IsAlive)
                .ToList();

            if (enemies.Count > 0)
            {
                // VFX
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
            }

            // 造成记录伤害
            await CreatureCmd.Damage(context.PlayerChoiceContext, enemies, recorded, ValueProp.Unpowered, Owner);

            // 消耗5点支援点数
            await PowerCmd.Apply<SupportPointPower>(context.PlayerChoiceContext, Owner, -5, null, null);

            // 重置记录伤害，进入追踪造成伤害阶段
            DynamicVars["RecordedDamage"].BaseValue = 0;
            DynamicVars["OutgoingDamage"].BaseValue = 0;

            isRecord = true;
            
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            // 如果处于爆发阶段，回合结束时造成记录伤害百分比的伤害
            if (isRecord)
            {
                int outgoing = DynamicVars["OutgoingDamage"].IntValue;
                int ratio = DynamicVars["BurstRatio"].IntValue;
                int burstDamage = outgoing * ratio / 100;

                if (burstDamage > 0)
                {
                    List<Creature> enemies = base.CombatState.Enemies
                        .Where(e => e != null && e.IsAlive)
                        .ToList();

                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies, burstDamage, ValueProp.Unpowered, Owner);
                }

                // 重置状态
                DynamicVars["OutgoingDamage"].BaseValue = 0;
                isRecord = false;
            }
        }
    }
}
