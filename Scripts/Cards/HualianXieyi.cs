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
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class HualianXieyi : MiyabiPartnerCardBase
    {
        public HualianXieyi() : base(2, CardRarity.Rare, TargetType.Self, CardType.Power)
        {
        }

        protected override string ArtPath => "res://images/cards/hualianXieyi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Strength", 2),
            new DamageVar(10,ValueProp.Unpowered),
            new DynamicVar(SupportVarName, 2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 获得2点力量
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].IntValue, Owner.Creature, this);

            // 每回合结束时对所有敌人造成伤害
            var power = await PowerCmd.Apply<HualianXieyiPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            power.SetDamage(DynamicVars.Damage.IntValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(5);
        }
    }
}
