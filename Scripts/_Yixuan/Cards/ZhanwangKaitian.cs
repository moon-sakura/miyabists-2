using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    /// <summary>
    /// 斩妄开天 - 2费Ancient攻击卡
    /// 立即给予所有敌人20%失衡易伤，自身每有一种正面能力，额外给予10%
    /// 舍弃所有手卡，抽取舍弃数量的卡，并发动对应次数的随机造成8点伤害
    /// </summary>
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ZhanwangKaitian : YixuanCardBase
    {
        public ZhanwangKaitian() : base(2, CardType.Attack,CardRarity.Ancient, TargetType.RandomEnemy)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/zhanwangKaitian.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8, ValueProp.Move),
            new DynamicVar("BaseDazeVuln", 20),
            new DynamicVar("BonusDazeVulnPerBuff", 10),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
            MiyabiKeywords.Friends,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        private static readonly string[] GuangStartVoices = { "guang_start_jiansuiwoyi", "guang_start_zhangjianzhuxie", "guang_start_zhanjinhongchen" };

        private static readonly string[] GuangEndVoices = { "guang_end_cijianpowang", "guang_end_songniyichneg", "guang_end_zhanduanjiuxiao" };

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            MiyabiAudioPlay.Random(GuangEndVoices);
            // 1. 计算自身正面能力（Buff）数量
            int buffCount = Owner.Creature.Powers
                .Count(p => p.Type == PowerType.Buff && p.Amount > 0);

            // 2. 给予所有敌人失衡易伤：基础20% + 每种正面能力10%
            int totalDazeVuln = DynamicVars["BaseDazeVuln"].IntValue
                + buffCount * DynamicVars["BonusDazeVulnPerBuff"].IntValue;

            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<DazeVulnPower>(choiceContext, enemy, totalDazeVuln, Owner.Creature, this);
            }

            // 3. 舍弃所有手牌
            var handCards = Owner.PlayerCombatState.Hand.Cards.ToList();
            int discardCount = handCards.Count;

            await CardCmd.DiscardAndDraw(choiceContext, handCards, discardCount);


            // 5. 对随机敌人造成8点伤害，发动对应抽卡次数
            int hitCount = discardCount;
            if (hitCount > 0)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .TargetingRandomOpponents(Owner.Creature.CombatState)
                    .WithHitCount(hitCount)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
