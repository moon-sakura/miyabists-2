using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class XingyueXiangsui : MiyabiPartnerCardBase
    {
        public XingyueXiangsui() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override string ArtPath => $"res://images/cards/xingyueXiangsui.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar(AnomalyBuildupVarName,1),
            new DynamicVar(SupportVarName,1),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<AnomalyBuildupPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

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

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, cardPlay.Target));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, Creature target)
        {
            if (!target.HasPower<AttributeAnomalyPower>()) return;
            var ano = target.Powers.OfType<AttributeAnomalyPower>().FirstOrDefault();
            await ano.DealAno(choiceContext, 1m);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out DynamicVar a)) a.UpgradeValueBy(1);
        }
    }
}
