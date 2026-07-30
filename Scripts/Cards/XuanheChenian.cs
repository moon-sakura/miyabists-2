using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 煊赫车辇 - 0费Ancient技能卡
    /// 喧响值不足20 → 回复5点喧响值
    /// 喧响值≥20 → 消耗20点喧响值，选择一个技能释放，消耗
    /// 升级后去掉消耗
    /// </summary>
    [RegisterCard(typeof(MiyabiCardPool))]
    internal class XuanheChenian : MiyabiCardBase
    {
        public XuanheChenian() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/cards/xuanheChenian.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust,
            MiyabiKeywords.Friends,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("DecibelCost", 20),
            new DynamicVar("DecibelRecover", 5),
            new DamageVar(8, ValueProp.Move),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<WanjunZhujue>(),
            HoverTipFactory.FromCard<KaixuanTantu>(),
            HoverTipFactory.FromCard<WujuJianshi>(),
            HoverTipFactory.FromCard<YongxianYouqiu>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // TODO: 获取当前喧响值
            var decibleRelics = Owner.Relics.OfType<IDecibleCounter>().FirstOrDefault();
            int currentDecibel = ((IDecibleCounter)decibleRelics).GetCounter(); // 占位，由用户完成
            int cost = DynamicVars["DecibelCost"].IntValue;

            if (currentDecibel < cost)
            {
                // 喧响值不足20 → 回复5点喧响值
                await MiyabiCombatService.AddDecible(Owner, DynamicVars["DecibelRecover"].IntValue);
            }
            else
            {
                MiyabiAudioPlay.Play("pp_gongzhicijian");

                // 喧响值≥20 → 消耗20点喧响值
                // TODO: 消耗喧响值的逻辑，由用户完成
                await MiyabiCombatService.AddDecible(Owner, -cost);

                // 创建四个技能选项（仅用于选择界面展示，不加入手卡）
                CardModel option1 = base.Owner.Creature.CombatState?.CreateCard<WanjunZhujue>(base.Owner);
                CardModel option2 = base.Owner.Creature.CombatState?.CreateCard<KaixuanTantu>(base.Owner);
                CardModel option3 = base.Owner.Creature.CombatState?.CreateCard<WujuJianshi>(base.Owner);
                CardModel option4 = base.Owner.Creature.CombatState?.CreateCard<YongxianYouqiu>(base.Owner);

                if (option1 != null && option2 != null && option3 != null && option4 != null)
                {
                    List<CardModel> options = new List<CardModel> { option1, option2, option3, option4 };

                    var result = Owner.Creature.CombatState.RunState.Rng.CombatCardSelection.NextItem<CardModel>(options);
                    options.Remove(result);

                    CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);

                    if (chosen is WanjunZhujue)
                    {
                        // 获得2点能量，抽2张牌
                        await PlayerCmd.GainEnergy(2, Owner);
                        await CardPileCmd.Draw(choiceContext, 2, Owner);
                    }
                    else if (chosen is KaixuanTantu)
                    {
                        // 获得2点力量，2点敏捷
                        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
                        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
                    }
                    else if (chosen is WujuJianshi)
                    {
                        // 每次使用攻击卡时，对随机敌人造成4点伤害
                        await PowerCmd.Apply<WujuJianshiPower>(choiceContext, Owner.Creature, 4, Owner.Creature, this);
                    }
                    else if (chosen is YongxianYouqiu)
                    {
                        // 对所有敌人造成8点伤害，击破则3倍并清除击破
                        foreach (var enemy in base.CombatState.HittableEnemies)
                        {
                            int damage = DynamicVars.Damage.IntValue;

                            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                                .FromCard(this, cardPlay)
                                .Targeting(enemy)
                                .Execute(choiceContext);

                            if (enemy.HasPower<BreakPower>())
                            {
                                await PowerCmd.Remove<BreakPower>(enemy);
                            }

                            //await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Move, Owner.Creature);

                        }
                    }
                }
            }
        }

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        {
            if(cardSource == this && target.HasPower<BreakPower>())
            {
                return 3m;
            }

            return 1m;
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
