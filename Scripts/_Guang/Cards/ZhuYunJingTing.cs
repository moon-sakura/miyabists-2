using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts._Guang.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 逐云惊霆 - 叶瞬光衍生技能卡（Token）
    /// 进入明心镜状态；青溟剑势不足 6 点时补至 6 点；回复所有能量，抽 3 张牌。
    /// 升级：额外将一张《斩妄开天》加入手卡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhuYunJingTing : GuangCardBase
    {
        public ZhuYunJingTing() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/zhuYunJingTing.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.EndSkill,
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MingxinThreshold", 6),
            new DynamicVar("DrawCount", 3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<MingxinJingPower>(),
            HoverTipFactory.FromCard<ZhanWangKaiTian>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 2. 青溟剑势不足 6 点则补至 6 点（已经有更多点时不削减）
            int threshold = DynamicVars["MingxinThreshold"].IntValue;
            int jianshi = Owner.Creature.GetPowerAmount<QingmingJianshiPower>();
            int delta = threshold - jianshi;
            if (delta > 0)
            {
                await PowerCmd.Apply<QingmingJianshiPower>(choiceContext, Owner.Creature,
                    delta, Owner.Creature, this);
            }

            // 3. 回复所有能量（把能量补满到本回合上限）
            //await PlayerCmd.SetEnergy(Owner.PlayerCombatState.MaxEnergy, Owner);
            int recoverEnergy = Owner.PlayerCombatState.MaxEnergy - Owner.PlayerCombatState.Energy;
            if (recoverEnergy > 0)
            {
                await PlayerCmd.GainEnergy(recoverEnergy, Owner);
            }
            // 1. 进入明心镜状态。
            //    变牌、消耗正面能力、扣生命上限、失衡易伤都由能力自己处理。
            if (!Owner.Creature.HasPower<MingxinJingPower>())
            {
                await PowerCmd.Apply<MingxinJingPower>(choiceContext, Owner.Creature,
                    1, Owner.Creature, this);
            }

            // 5. 升级后：额外将一张《斩妄开天》加入手卡
            if (IsUpgraded)
            {
                CardModel kaiTian = Owner.Creature.CombatState.CreateCard<ZhanWangKaiTian>(Owner);
                if (kaiTian != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(kaiTian, PileType.Hand, Owner);
                }
            }

            // 4. 抽牌（明心镜状态下抽到的卡会自动变为明心镜卡）
            await CardPileCmd.Draw(choiceContext, DynamicVars["DrawCount"].IntValue, Owner);

        }

        protected override void OnUpgrade()
        {
            // 升级效果由 OnPlay 里的 IsUpgraded 分支处理：
            // 额外将一张《斩妄开天》加入手卡。
        }
    }
}
