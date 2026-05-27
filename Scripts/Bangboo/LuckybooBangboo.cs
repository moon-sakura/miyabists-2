using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class LuckybooBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/luckyboo.tscn";

        public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            await base.OnSummon(owner, self, options); // 先调用基类的 OnSummon 来设置血量等基础属性

            //base.IsHealthBarVisible = true;
            if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                await PowerCmd.Apply<LuckybooAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }

    }

    internal class LuckybooAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.AnyEnemy;
        public override string BigIconPath => "res://images/bangboo/relicMode/luckybooRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar ("MAXUSE", MAXUSE),
            new DynamicVar("Used",0),
            new DamageVar(20m,ValueProp.Move)
        ];

        protected override bool CanPayCost => Owner.PetOwner.Gold >= 10m;

        public override async Task ActCost()
        {
            await PlayerCmd.LoseGold(10m, Owner.PetOwner);
            string sfx = "event:/sfx/ui/gold/gold_1";
            SfxCmd.Play(sfx);
            UsedCount++;
            DynamicVars["Used"].BaseValue = UsedCount;
        }

        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            await MinionAnimCmd.PlayBumpAttackAsync(Owner, target);
            await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage, Owner);
        }
    }
}
