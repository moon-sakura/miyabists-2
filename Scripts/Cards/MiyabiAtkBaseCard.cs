using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 雅组通用攻击基类：处理 伤害 + 冰焰 + 失衡 + 烈霜积蓄 逻辑
    /// </summary>
    internal abstract class MiyabiAttackCardBase : MiyabiCardBase
    {
        protected const string DazeVarName = "DAZE_POWER";

        protected MiyabiAttackCardBase(int energy, CardRarity rarity, TargetType target, bool showInLib)
            : base(energy, CardType.Attack, rarity, target, showInLib)
        {
        }

        // 默认所有此类卡牌都带有烈霜词条
        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.LieShuang];

        // 通用打出逻辑
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            // 1. 执行基础攻击
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }

        // 通用伤害后逻辑：将伤害转化为烈霜积蓄值
        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (cardSource != this || target == null || target.IsDead) return;
            int chkFB = target.GetPowerAmount<FrostBuildPower>() + result.TotalDamage;

            // 确保是本卡造成的实际伤害，且目标存活
            if (result.TotalDamage > 0 && chkFB <= 100)
            {
                // 如果拥有烈霜词条，按伤害量施加积蓄值
                if (this.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang) && !target.HasPower<FrostPower>())
                {
                    await PowerCmd.Apply<FrostBuildPower>(target, result.TotalDamage, base.Owner.Creature, this);
                }
            }
            //烈霜积蓄值积攒逻辑
            if (chkFB >= 101)
            {
                await PowerCmd.SetAmount<FrostBuildPower>(target, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<FrostPower>(target, 1, base.Owner.Creature, this);
                if (target.HasPower<AttributeAnomalyPower>())
                {
                    await MiyabiCombatService.DisorderApply(target, base.Owner.Creature,choiceContext);
                }
                else
                {
                    await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, base.Owner.Creature, this);
                }
            }

            // 2. 施加 1 层冰焰 (FrostFirePower)
            if (chkFB <= 100 && this.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang))
                await PowerCmd.Apply<FrostFirePower>(target, 1, base.Owner.Creature, this);

            // 3. 施加动态配置的失衡值 (DazePower)
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar))
            {
                await MiyabiCombatService.AddDaze(target, dazeVar, base.Owner.Creature);
            }

        }
    }
}