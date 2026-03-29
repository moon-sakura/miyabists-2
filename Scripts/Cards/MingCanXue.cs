using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class MingCanXue : MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/mingCanxue.png";


        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.LieShuang,
            MiyabiKeywords.EndSkill,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        public MingCanXue() : base(1, CardRarity.Token, TargetType.AllEnemies, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount",5)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            int amount = CombatManager.Instance.History.CardPlaysFinished
                .Count((CardPlayFinishedEntry e)
                => e.CardPlay.Card.Type == CardType.Attack
                && e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState));

            int num = 0;
            if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc))
                num = hc.IntValue + amount;

            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                Color color = new Color("FFFFFF80");
                double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + (double)Mathf.Min(8, num) * num2));
                //SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
                NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
            }
            //if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc))
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

            int add = num / 2;
            await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature, add, null, null);
        }


        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
    }
}
