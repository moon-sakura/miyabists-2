using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class HuaXinFeng : MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public HuaXinFeng() : base(1, CardRarity.Common, TargetType.AnyEnemy, true) { }


        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 2)
        ];


        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CheckReduce();
        }

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card == this)
            {
                await CheckReduce();
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }

        private async Task CheckReduce()
        {
            int amount = CombatManager.Instance.History.CardPlaysFinished
                .Count((CardPlayFinishedEntry e)
                => e.CardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                && e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState));
            if (base.Owner != null && base.Owner.Creature.IsAlive)
            {
                if (amount > 0)
                    SetCost(0);
            }
        }

        private void ReduceCostBy(int amount)
        {
            base.EnergyCost.AddThisTurn(-amount);
        }

        private void SetCost(int a)
        {
            base.EnergyCost.SetThisTurn(a);
        }
    }
}
