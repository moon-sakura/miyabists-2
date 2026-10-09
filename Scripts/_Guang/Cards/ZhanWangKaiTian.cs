using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
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
    /// 斩妄开天 - 叶瞬光衍生攻击卡（Token，由《逐云惊霆》升级后加入手卡）
    /// 对所有敌人造成 66 点伤害，退出明心镜状态，永久获得 10 点最大生命值。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class ZhanWangKaiTian : GuangMingxinjingCardBase
    {
        public ZhanWangKaiTian() : base(2, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.EndSkill,
            CardKeyword.Exhaust,
            CardKeyword.Ethereal,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(66, ValueProp.Move),
            new DynamicVar("MaxHpGain", 10),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MingxinJingPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .TargetingAllOpponents(Owner.Creature.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 退出明心镜状态（能力被移除时会把所有明心镜卡变回原卡）
            if (Owner.Creature.HasPower<MingxinJingPower>())
            {
                await PowerCmd.Remove(Owner.Creature.GetPower<MingxinJingPower>());
            }

            // 永久获得最大生命值（战斗中提升，战斗结束后依然保留）
            await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars["MaxHpGain"].IntValue);
        }

        protected override void OnUpgrade()
        {
            // 这张卡由《逐云惊霆》升级后生成，本身暂不支持升级。
            DynamicVars.Damage.UpgradeValueBy(33);
            DynamicVars["MaxHpGain"].UpgradeValueBy(2);
        }
    }
}
