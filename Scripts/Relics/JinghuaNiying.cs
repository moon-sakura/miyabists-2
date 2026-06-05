using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class JinghuaNiyingRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override string PackedIconPath => "res://images/relics/jinghuaNiying.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<JixianShiyu>(),
            //HoverTipFactory.FromPower<FrostFallPower>(),
            //HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill)
        ];

        private bool isTriggered = false;

        public override async Task BeforeAttack(AttackCommand command)
        {
            if (isTriggered || !command.Attacker.IsMonster || command.TargetSide != base.Owner.Creature.Side) return;
            //if (!command.Results.Any(r => r.Receiver == base.Owner.Creature //攻击目标是自己
            //&& (r.TotalDamage > base.Owner.Creature.Block))) //攻击结果是伤害未被格挡了
            //{ return; }

            CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<JixianShiyu>(base.Owner.Creature.Player);
            await CardCmd.AutoPlay(new HookPlayerChoiceContext(base.Owner, base.Owner.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any), reward1, base.Owner.Creature);

            isTriggered = true;
        }

        //public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        //{
        //    if (target != base.Owner.Creature || isTriggered || !dealer.IsMonster)
        //    {
        //        return Task.CompletedTask;
        //    }

        //    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<JixianShiyu>(base.Owner.Creature.Player);
        //    CardCmd.AutoPlay(choiceContext, reward1, base.Owner.Creature);

        //    isTriggered = true;

        //    return Task.CompletedTask;
        //}

        public override Task AfterCombatEnd(CombatRoom room)
        {
            isTriggered = false;
            return base.AfterCombatEnd(room);
        }
    }
}
