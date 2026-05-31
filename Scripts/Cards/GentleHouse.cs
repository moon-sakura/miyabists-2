using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Miyabists2.Scripts.Enemies;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class GentleHouse : MiyabiCardBase
    {
        protected override string ArtPath => $"res://images/cards/miyabiDuel.png";
        public GentleHouse() : base(1, CardType.None, CardRarity.None, TargetType.AnyEnemy, false) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [

        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromCard<HuaCi>(),
        ];

        private bool isPlaying = false;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            try
            {
                await SetPlaying(true);
                var target = cardPlay.Target;
                await CreatureCmd.Kill(target);
                await Cmd.Wait(0.2f);
                MonsterModel monster = ModelDb.Monster<MiyabiBoss>().ToMutable();
                await CreatureCmd.Add(monster, target.CombatState, target.Side, target.SlotName);
            }
            finally
            {
                await SetPlaying(false);
            }
        }

        public async Task SetPlaying(bool v)
        {
            isPlaying = v;
        }

        public override bool ShouldStopCombatFromEnding()
        {
            return isPlaying;
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
