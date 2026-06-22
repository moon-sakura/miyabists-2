using STS2RitsuLib.Interop.AutoRegistration;

using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class HuaCi : MiyabiAttackCardBase
    {
        protected override string ArtPath => $"res://images/cards/huaCi.png";

        public HuaCi() : base(0, CardRarity.Token, TargetType.AnyEnemy, true) { }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.LieShuang,
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new BlockVar(3, ValueProp.Move),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            if (!isAOE)
                // 1. 执行基础攻击
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);

            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        }

        //public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        //{
        //    int handSize = base.Owner.PlayerCombatState.Hand.Cards.Count;

        //    if (card == this)
        //    {
        //        if(handSize >= 10)
        //        {
        //            var target = base.CombatState.HittableEnemies.TakeRandom(1, base.Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        //            await CardCmd.AutoPlay(choiceContext, this, target);
        //        }
        //    }
        //}

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            DynamicVars.Block.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
        }
    }
}
