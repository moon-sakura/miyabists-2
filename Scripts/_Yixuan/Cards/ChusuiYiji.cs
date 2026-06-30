using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ChusuiYiji : YixuanAtkCardBase
    {
        public ChusuiYiji() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/chuhuiYiji.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar(ShannengVarName, 5),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 恢复闪能
            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, DynamicVars[ShannengVarName].IntValue, Owner.Creature, this);
        }

        //public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        //{
        //    if(power is ShannengPower && power.Owner == Owner.Creature && amount < 0)
        //    {
        //        var enemy = Owner.Creature.CombatState.Enemies.TakeRandom(1, Owner.Creature.CombatState.RunState.Rng.Shuffle).FirstOrDefault();
        //        await CardCmd.AutoPlay(choiceContext,this,enemy);
        //    }
            
        //}

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
        }
    }
}
