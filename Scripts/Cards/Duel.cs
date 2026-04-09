using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.Performance;

namespace Miyabists2.Scripts.Cards
{
    internal class MiyabiDuel : MiyabiCardBase
    {
        public MiyabiDuel() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var duelAttack = new MoveState(
                "MIYABI_DUEL_ATTACK",
                async targets => await DamageCmd // 意图2实际执行效果，这里直接用lambda
                    .Attack(10)
                    //.Targeting(base.Owner.Creature)
                    .FromMonster(cardPlay.Target.Monster)
                    //.WithAttackerFx()
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(null),
                new SingleAttackIntent(10)
            )
            {
                FollowUpStateId = cardPlay.Target.Monster.NextMove.StateId,
                MustPerformOnceBeforeTransitioning = true
            };

            cardPlay.Target.Monster.SetMoveImmediate(duelAttack, true);

            await PowerCmd.Apply<MiyabiParryPower>(base.Owner.Creature, 1, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
