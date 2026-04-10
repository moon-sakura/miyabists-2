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
    internal class CiquanLianji : MiyabiPartnerCardBase
    {
        //public override string PortraitPath => $"res://images/cards/baojunMengji.png";

        public CiquanLianji() : base(2, CardRarity.Uncommon, TargetType.AnyEnemy, CardType.Attack) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(2, ValueProp.Move),
            new DynamicVar(DazeVarName, 12),
            new DynamicVar("LieshuangUp", 50),
            new DynamicVar("HitCount", 3),
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
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
                    .FromCard(this)
                    .WithHitCount(DynamicVars["HitCount"].IntValue)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }

            if (base.CheckSupportCost(2) != 0)
            {
                if(base.DynamicVars.TryGetValue("LieshuangUp", out DynamicVar u))
                {
                    u.BaseValue += 25;
                }

                if (cardPlay.Target.HasPower<CiquanljPower>())
                {
                    int c = cardPlay.Target.GetPowerAmount<CiquanljPower>();
                    {
                        if(u.IntValue > c)
                        {
                            await PowerCmd.Apply<CiquanljPower>(cardPlay.Target, u.IntValue - c, base.Owner.Creature, this);
                            var p = cardPlay.Target.Powers.OfType<CiquanljPower>().FirstOrDefault();
                            p.ResetCount();
                        }
                        else if(u.BaseValue == c)
                        {
                            var p = cardPlay.Target.Powers.OfType<CiquanljPower>().FirstOrDefault();
                            p.ResetCount();
                        }
                        else
                        {
                            u.BaseValue = 50;
                            return;
                        }
                    }
                }

                await PowerCmd.Apply<CiquanljPower>(cardPlay.Target, DynamicVars["LieshuangUp"].IntValue, base.Owner.Creature, this);
                
                await CostSupporPoint(2);
            }

        }



        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);

        }
    }
}
