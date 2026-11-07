using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 剑光惊鸿 - 罕见技能卡
    /// 消耗所有青溟剑势，升级你的全部卡牌。
    /// 每消耗 3 点青溟剑势，获得 1 点能量。
    /// 消耗。
    /// 升级：每消耗 2 点青溟剑势即可获得 1 点能量。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class JianGuangJingHong : GuangCardBase
    {
        public JianGuangJingHong() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("JianshiPerEnergy", 3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 1. 消耗所有青溟剑势
            int jianshi = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
            if (jianshi > 0)
            {
                await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                    -jianshi, Owner.Creature, this);
            }

            // 2. 每消耗 N 点青溟剑势获得 1 点能量
            int perEnergy = DynamicVars["JianshiPerEnergy"].IntValue;
            if (perEnergy > 0)
            {
                int energy = jianshi / perEnergy;
                if (energy > 0)
                {
                    await PlayerCmd.GainEnergy(energy, Owner);
                }
            }

            // 3. 升级你的全部卡牌（本身正在结算，跳过）
            List<CardModel> allCards = Owner.PlayerCombatState.AllCards.ToList();
            foreach (CardModel card in allCards)
            {
                if (card == this || !card.IsUpgradable) continue;

                CardCmd.Upgrade(card);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["JianshiPerEnergy"].UpgradeValueBy(-1); // 升级后 2 点换 1 点能量
        }
    }
}
