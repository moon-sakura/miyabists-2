using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    internal class BaojunMengji : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/baojunMengji.png";

        public BaojunMengji() : base(2,CardRarity.Uncommon, TargetType.AnyEnemy) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new BlockVar(12,ValueProp.Move),
            new DynamicVar(ParryVarName, 2),
            new DynamicVar(SupportVarName,2),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<MiyabiParryPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, 4, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
            DynamicVars.Block.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(ParryVarName, out DynamicVar p)) p.UpgradeValueBy(1);

        }
    }
}

