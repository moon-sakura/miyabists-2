using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class BingyuanQianxi : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/bingyuanQianxi.png";
        public BingyuanQianxi() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8, ValueProp.Move),
            new DynamicVar(DazeVarName, 6),
            new DynamicVar("Buffer",6),
            new DynamicVar(SupportVarName,1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<SlipperyPower>(),
            HoverTipFactory.FromPower<MiyabiParryPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }
   
            //await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext));
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if(cardSource != this) return;

            await base.AfterDamageGiven(choiceContext, dealer, result, props, target, cardSource);

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, result.TotalDamage));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, decimal damage)
        {
            decimal bufferAmount = damage / DynamicVars["Buffer"].BaseValue;
            await PowerCmd.Apply<SlipperyPower>(choiceContext, base.Owner.Creature, bufferAmount, Owner.Creature, this);
            await PowerCmd.Apply<MiyabiParryPower>(choiceContext, base.Owner.Creature, bufferAmount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.BaseValue += 4;
            DynamicVars[DazeVarName].BaseValue += 2;
            DynamicVars["Buffer"].BaseValue -= 1;
        }
    }
}
