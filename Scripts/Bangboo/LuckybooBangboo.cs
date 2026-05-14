using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class LuckybooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/luckyboo.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(choiceContext, owner, self, options); // 先调用基类的 OnSummon 来设置血量等基础属性

            //base.IsHealthBarVisible = true;
            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<LuckybooAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }

        public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
        {
            if (side != base.Creature.Side || base.Creature.IsDead || base.Creature.HasPower<LuckybooAct>())
                return;

            await PowerCmd.Apply<LuckybooAct>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        }
    }

    internal class LuckybooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyEnemy;

        protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        {
            if (Owner.PetOwner.Gold < 10m)
                return;

            await PlayerCmd.LoseGold(10m, Owner.PetOwner);
            await CreatureCmd.Damage(choiceContext, target, 20m, ValueProp.Move, Owner);
        }
    }
}
