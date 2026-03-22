using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(MiyabiCardPool))]
    internal class MiyabiBlock : MiyabiCardBase
    {
        // 基础耗能
        private const int energyCost = 1;
        // 卡牌类型
        private const CardType type = CardType.Attack;
        // 卡牌稀有度
        private const CardRarity rarity = CardRarity.Basic;
        // 目标类型（AnyEnemy表示任意敌人）
        private const TargetType targetType = TargetType.AnyEnemy;
        // 是否在卡牌图鉴中显示
        private const bool shouldShowInCardLibrary = true;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.LieShuang];

        // 卡牌的基础属性
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(4, ValueProp.Move),
            new DynamicVar("PARRY_POWER", 1) 
        ];

        // 指定卡牌立绘路径
        public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public MiyabiBlock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
        {
        }

        // 打出时的效果逻辑
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
                .FromCard(this) // 伤害来源于这张卡牌
                .Targeting(cardPlay.Target) // 伤害目标是玩家选择的目标
                .Execute(choiceContext);

            base.DynamicVars.TryGetValue("PARRY_POWER", out DynamicVar value);
            await PowerCmd.Apply<DazePower>(cardPlay.Target, value.BaseValue, base.Owner.Creature, this);

        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (target != null && !target.IsDead && result.TotalDamage > 0 && cardSource == this)
            {
                if (cardSource.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang))
                    await PowerCmd.Apply<FrostBuildPower>(target, result.TotalDamage, base.Owner.Creature, this);
            }
        }

        // 升级后的效果逻辑
        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2); // 升级后增加4点伤害
            if (base.DynamicVars.TryGetValue("PARRY_POWER", out DynamicVar value))
            {
                //value?.UpgradeValueBy(1);
            }
        }
    }
}
