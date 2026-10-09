using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 掣惊雷 - 罕见攻击卡
    /// 随机对敌人造成3次4点伤害，失衡4。
    /// 附加：被消耗时抽1张卡，抽到的卡本回合免费使用。
    /// 升级：4次。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class CheJingLei : GuangCardBase, I2MingxinJing
    {
        public CheJingLei() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/cheJingLei.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(4, ValueProp.Move),
            new DynamicVar(DazeVarName, 4),
            new DynamicVar("HitCount", 3),
            new CardsVar(1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int hitCount = DynamicVars["HitCount"].IntValue;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .WithHitCount(hitCount)
                .TargetingRandomOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            //foreach (Creature enemy in Owner.Creature.CombatState.HittableEnemies)
            //{
            //    await MiyabiCombatService.AddDaze(choiceContext, enemy, DynamicVars[DazeVarName], base.Owner.Creature);
            //}
        }

        protected override void OnUpgrade()
        {
            DynamicVars["HitCount"].UpgradeValueBy(1); // 升级后4次
        }

        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            var drawnCard = await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
            if(drawnCard.Count() > 0)
            {
                foreach (var card in drawnCard)
                {
                    card.SetToFreeThisTurn();
                }
            }
        }

            // ========== 待实现 ==========
            // TODO: 被消耗时抽 DynamicVars.Cards 张卡，抽到的卡本回合免费使用
            // 需要挂接本卡被消耗的时机回调（抽到后 SetToFreeThisTurn()）。
    }
}
