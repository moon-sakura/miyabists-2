using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
        internal class LuoyuShenghua : MiyabiPartnerCardBase
    {

        public override string PortraitPath => $"res://images/cards/luoyuShenghua.png";
        public LuoyuShenghua() : base(1, CardRarity.Uncommon, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("LuoYu", 3),
            new DynamicVar(SupportVarName,1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<AnomalyBuildupPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue("LuoYu", out DynamicVar v))
                await PowerCmd.Apply<LuoyushPower>(choiceContext, base.Owner.Creature, v.BaseValue, base.Owner.Creature, this);


            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext));

        }

        async Task FriendFunc(PlayerChoiceContext choiceContext)
        {
            foreach (Creature Enemy in base.CombatState.Enemies)
            {
                if (Enemy != null && Enemy.IsAlive)
                {
                    await MiyabiCombatService.AddAnoBuildup(Enemy, 1, Owner.Creature, this, choiceContext);
                }
            }
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("LuoYu", out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
