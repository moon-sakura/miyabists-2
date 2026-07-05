using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(MiyabiCardPool))]
    //[RegisterCard(typeof(StatusCardPool))]
    internal class ShuangYue:MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/shuangYue.png";

        //private int _atkCount = 0;
        public override int MaxUpgradeLevel => 100;


        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            MiyabiKeywords.LieShuang,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<FrostFallPower>(),
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
            HoverTipFactory.FromPower<FrostPower>(),
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>()
        ];

        public ShuangYue() : base(0, CardRarity.Token, TargetType.AllEnemies, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 2),
            new DynamicVar("HitCount",2)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            //await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature,-2,null,this);

            //await GetCostCount();

            int num = 0;
            if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc))
                num = hc.IntValue;

            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                Color color = new Color("FFFFFF80");
                double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + (double)Mathf.Min(8, num) * num2));
                //SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
                NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
            }
            //if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc))
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this, cardPlay)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

            if (base.Owner.Creature.GetPower<FrostFallPower>().DisplayAmount >= 2)
            {
                // 加入一张升级后的《霜月》到手中
                ShuangYue reward1 = base.Owner.Creature.CombatState.CreateCard<ShuangYue>(base.Owner.Creature.Player);
                int targetLevel = base.CurrentUpgradeLevel + 1;
                for (int i = 0; i < targetLevel; i++)
                {
                    reward1.UpgradeInternal();
                }
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                await PowerCmd.Apply<FrostFallPower>(choiceContext, base.Owner.Creature, -2, null, null);
            }
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Creature.Side) return;
            CardCmd.Downgrade(this);
        }


        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc)) hc.UpgradeValueBy(1);
            
            if(base.CurrentUpgradeLevel >= 3)
            {
                EnergyCost.SetThisTurn(1);
            }
        }


        private async Task GetCostCount()
        {
            //_atkCount = base.Owner.GetRelic<SwordNotailRelic>().LuoShuangCostThisTurn;
        }
    }
}
