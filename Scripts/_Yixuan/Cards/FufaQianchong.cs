using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class FufaQianchong : YixuanAtkCardBase
    {
        public FufaQianchong() : base(0, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar("HitCount",4),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Xuanmo,
            MiyabiKeywords.EndSkill,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["HitCount"].IntValue).FromCard(this)
            .Unblockable()
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

            //foreach (var target in CombatState.HittableEnemies)
                //await PowerCmd.Apply<ShufaZhi>(choiceContext, target, 20, Owner.Creature, this);

            CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<XuanmoJizhen>(base.Owner.Creature.Player);
            reward1.SetToFreeThisTurn();
            reward1.AddKeyword(CardKeyword.Exhaust);
            await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
