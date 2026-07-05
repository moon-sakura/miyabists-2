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
        internal class CiquanLianji : MiyabiPartnerCardBase
    {
        protected override string ArtPath => $"res://images/cards/ciquanLianji.png";

        public CiquanLianji() : base(2, CardRarity.Rare, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(2, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new DynamicVar("LieshuangUp", 50),
            new DynamicVar("HitCount", 3),
            new DynamicVar(SupportVarName,3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .WithHitCount(DynamicVars["HitCount"].IntValue)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, cardPlay.Target));

        }

        async Task FriendFunc(PlayerChoiceContext choiceContext, Creature target)
        {
            if (base.DynamicVars.TryGetValue("LieshuangUp", out DynamicVar u))
            {
                u.BaseValue += 25;
            }

            if (target.HasPower<CiquanljPower>())
            {
                int c = target.GetPowerAmount<CiquanljPower>();
                {
                    if (u.IntValue > c)
                    {
                        await PowerCmd.Apply<CiquanljPower>(choiceContext, target, u.IntValue - c, base.Owner.Creature, this);
                        var p = target.Powers.OfType<CiquanljPower>().FirstOrDefault();
                        p.ResetCount();
                    }
                    else if (u.BaseValue == c)
                    {
                        var p = target.Powers.OfType<CiquanljPower>().FirstOrDefault();
                        p.ResetCount();
                    }
                    else
                    {
                        u.BaseValue = 50;
                        return;
                    }
                }
            }
            await PowerCmd.Apply<CiquanljPower>(choiceContext, target, DynamicVars["LieshuangUp"].IntValue, base.Owner.Creature, this);

        }



        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);

        }
    }
}
