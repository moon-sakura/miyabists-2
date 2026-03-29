using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class MingfuWange : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/mingfuWange.png";
        public MingfuWange() : base(2, CardRarity.Rare, TargetType.AnyEnemy,CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 15),
            new DynamicVar("WanGe",3)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<XiezouJusha>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }


            if (base.DynamicVars.TryGetValue("WanGe", out DynamicVar w))
                await PowerCmd.Apply<MingfuwgPower>(base.Owner.Creature, w.BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<DazeVulnPower>(cardPlay.Target, 35, base.Owner.Creature, this);
            if (base.CheckSupportCost(1) != 0)
            {
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<XiezouJusha>(base.Owner.Creature.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
                await CostSupporPoint(1);
            }
        }
        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(5);
            if (base.DynamicVars.TryGetValue("WanGe", out DynamicVar w)) w.UpgradeValueBy(2);
        }
    }
}
