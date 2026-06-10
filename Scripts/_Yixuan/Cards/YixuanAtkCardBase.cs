using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Cards;
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
    public abstract class YixuanAtkCardBase : MiyabiCardBase
    {
        public YixuanAtkCardBase(int baseCost, CardRarity rarity, TargetType target, CardType type = CardType.Attack, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Xuanmo];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.Xuanmo),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
                .Unblockable()
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (cardSource != this || target == null || target.IsDead) return;

            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar))
            {
                await MiyabiCombatService.AddDaze(choiceContext, target, dazeVar, base.Owner.Creature);
            }

            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, 1m, base.Owner.Creature, this);
        }
    }
}
