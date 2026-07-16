using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MinionLib.Targeting;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Service;
using Steamworks;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class BangbooUseOnemore : MiyabiCardBase, IModRightClickableCard
    {
        protected override string ArtPath => $"res://images/cards/bangbooBrust.png";

        public BangbooUseOnemore() : base(0, CardType.Skill, CardRarity.Uncommon, MinionTargetTypes.AnyMinion) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            return !MiyabiCombatService.IsBangbooOnField(Owner) && Owner.PlayerCombatState.Energy >= 1;
        }

        // 右键执行（多人下会在所有客户端同步执行）
        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            await CardCmd.Discard(context.PlayerChoiceContext, this);
            await PlayerCmd.GainEnergy(-1, Owner);
            await MiyabiCombatService.SummonBangbooRandom(context.PlayerChoiceContext, Owner);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if(cardPlay.Target == Owner.Creature || cardPlay.Target == null)
            {
                await MiyabiCombatService.SummonBangbooRandom(choiceContext, Owner);
                return;
            }

            if (cardPlay.Target is not { Monster: MiyabiBangbooBase } target) return;

            var act = target.Powers.Where(p => p is MiyabiBangbooActBase).FirstOrDefault();

            ((MiyabiBangbooActBase)act).MAXUSE++;            
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            base.OnUpgrade();
        }
    }
}
