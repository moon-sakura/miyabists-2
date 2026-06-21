using STS2RitsuLib.Utils;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Relics;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
        internal class TestCardFrost : MiyabiCardBase
    {
        public TestCardFrost()
            : base(0, CardType.Attack, CardRarity.None, TargetType.AnyEnemy, false)
        {
        }


        // 閫氱敤鎵撳嚭閫昏緫
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            Creature target = cardPlay.Target;
            ArgumentNullException.ThrowIfNull(target, "cardPlay.Target");
            if (cardPlay.Card != this || target == null || target.IsDead) return;
            int chkFB = target.GetPowerAmount<FrostBuildPower>() + 30;

            MiyabiCombatService.SetFrostTriggerMultiply(base.Owner.Creature);
            int trigger = MiyabiCombatService.GetFrostTrigger();

            if (chkFB <= trigger && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
            {
                await PowerCmd.Apply<FrostBuildPower>(choiceContext, target, 30, base.Owner.Creature, this);

            }
            if (chkFB >= trigger + 1 && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
            {
                await MiyabiCombatService.FrostApply(target, Owner.Creature, choiceContext);
            }
        }
    }
}
