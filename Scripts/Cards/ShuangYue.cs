using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    internal class ShuangYue:MiyabiAttackCardBase
    {
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        private int _atkCount = 0;

        public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            MiyabiKeywords.LieShuang,
            CardKeyword.Exhaust,
            CardKeyword.Retain
        ];

        public ShuangYue() : base(0, CardRarity.Token, TargetType.AllEnemies, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new DynamicVar(DazeVarName, 2)
            //new DynamicVar("HitCount",0)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            //if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc)) 
            //    hc.BaseValue = base.Owner.Creature.GetPower<FrostFallPower>().CostThisTurn;
            //await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature,-2,null,this);
            
            await GetCostCount();

            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount(_atkCount)
                .FromCard(this).TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);

            if(base.Owner.Creature.GetPower<FrostFallPower>().DisplayAmount >= 2)
            {
                // 加入一张《霜月》到手中
                CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<ShuangYue>(base.Owner.Creature.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);
                await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature, -2, null, null);
            }
        }


        protected override void OnUpgrade()
        {
            //DynamicVars.Damage.UpgradeValueBy(3);
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(1);
            //if (base.DynamicVars.TryGetValue("HitCount", out DynamicVar hc)) hc.UpgradeValueBy(1);
        }

        private async Task GetCostCount()
        {
            _atkCount = base.Owner.GetRelic<SwordNotailRelic>().LuoShuangCostThisTurn;
        }
    }
}
