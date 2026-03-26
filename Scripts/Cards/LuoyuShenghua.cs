using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class LuoyuShenghua : MiyabiPartnerCardBase
    {
        public LuoyuShenghua() : base(1, CardRarity.Uncommon, TargetType.Self, CardType.Power) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("LuoYu", 3),
            new DamageVar(0,ValueProp.Unpowered),
            new BlockVar(0,ValueProp.Unpowered)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue("LuoYu", out DynamicVar v))
                await PowerCmd.Apply<LuoyushPower>(base.Owner.Creature, v.BaseValue, base.Owner.Creature, this);
            if (base.CheckSupportCost(1) != 0)
            {
                foreach (Creature Enemy in base.CombatState.Enemies)
                {
                    if (Enemy != null && Enemy.IsAlive) 
                    {
                        await PowerCmd.Apply<AnomalyBuildupPower>(Enemy, 1, base.Owner.Creature, this); ;
                    }
                    //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
                }
                await CostSupporPoint(1);
            }

        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("LuoYu", out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
