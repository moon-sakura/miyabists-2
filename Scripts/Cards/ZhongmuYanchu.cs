using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
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
        internal class ZhongmuYanchu : MiyabiPartnerCardBase
    {
        public override string PortraitPath => $"res://images/cards/zhongmuYanchu.png";

        public ZhongmuYanchu() : base(2, CardRarity.Rare, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(3, ValueProp.Move),
            new DynamicVar(DazeVarName, 10),
            new DynamicVar(AnomalyBuildupVarName, 3),
            new DynamicVar(SupportVarName,3),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
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
                    .WithHitCount(3)
                    .Execute(choiceContext);
            }

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext,cardPlay.Target));
        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, Creature target)
        {
            await PowerCmd.Apply<ZhongmuycPower>(choiceContext, target, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(3);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(15);
            if (base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out DynamicVar a)) a.UpgradeValueBy(1);

        }
    }
}
