using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Commands;
using MinionLib.Minion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class AgentBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/agent.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(choiceContext, owner, options); // 先调用基类的 OnSummon 来设置血量等基础属性

            //base.IsHealthBarVisible = true;
            await PowerCmd.Apply<AgentAct>(new ThrowingPlayerChoiceContext(), this.Creature, 1m, owner.Creature, options.Source);
        }
    }

    internal class AgentAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyEnemy;
        public override string BigIconPath => "res://images/bangboo/relicMode/agentGulliverRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar ("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
            new DynamicVar("DamageUp",30m),
            new DamageVar(8m,ValueProp.Move)
        ];

        private List<Creature> _monsterModels = new List<Creature>();

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            await MinionAnimCmd.PlayBumpAttackAsync(Owner, target);
            await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage, Owner);
            _monsterModels.Add(target);
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (_monsterModels.Any(c => c == target))
            {
                return 1m + DynamicVars["DamageUp"].BaseValue / 100m;
            }
            return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            _monsterModels.Clear();
            return base.AfterPlayerTurnStart(choiceContext, player);
        }
    }
}
