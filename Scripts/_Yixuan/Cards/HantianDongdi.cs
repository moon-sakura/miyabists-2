using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Patches;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class HantianDongdi : YixuanPartnerCardBase
    {
        public HantianDongdi() : base(3, CardRarity.Rare, TargetType.AnyEnemy, CardType.Attack)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/hantianDongdi.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(1, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount", 7),
            new DynamicVar(SupportVarName, 3),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            MiyabiKeywords.Mingpo,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int currentVigor = Owner.Creature.GetPowerAmount<VigorPower>();

            await base.OnPlay(choiceContext, cardPlay);

            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Unblockable()
                .WithHitCount(DynamicVars["HitCount"].IntValue)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            // 支援点数3：获得原有层数活力
            await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
            {
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, currentVigor, Owner.Creature, this);
            });
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
            AddKeyword(CardKeyword.Retain);
        }
    }
}
