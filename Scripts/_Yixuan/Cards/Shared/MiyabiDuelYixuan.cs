using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class MiyabiDuelYixuan : MiyabiDuel
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ThornsPower>(),
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("THORNS_POWER", 2),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var duelAttack = new MoveState(
                "YIXUAN_DUEL_ATTACK",
                async targets => await DamageCmd
                    .Attack(1)
                    .WithHitCount(4)
                    //.Targeting(base.Owner.Creature)
                    .FromMonster(cardPlay.Target.Monster)
                    //.WithAttackerFx()
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(null),
                new MultiAttackIntent(1, 4)
            )
            {
                FollowUpStateId = cardPlay.Target.Monster.NextMove.StateId,
                MustPerformOnceBeforeTransitioning = true
            };

            cardPlay.Target.Monster.SetMoveImmediate(duelAttack, true);

            await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner.Creature, 2m, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["THORNS_POWER"].UpgradeValueBy(1);
            AddKeyword(CardKeyword.Retain);
        }
    }
}
