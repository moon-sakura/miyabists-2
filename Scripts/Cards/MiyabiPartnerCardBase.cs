using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;


namespace Miyabists2.Scripts.Cards
{
    internal abstract class MiyabiPartnerCardBase : MiyabiCardBase
    {
        // 伙伴卡通常消耗的支援点数变量名
        protected int _supportCost = 0; // 默认需要 0 点支援点数

        protected const string DazeVarName = "DAZE_POWER";
        protected const string ParryVarName = "PARRY_POWER";
        protected const string SlipperyVarName = "SLIPPERY_POWER";
        protected const string AnomalyBuildupVarName = "ANOBUILD_POWER";

        protected const bool isDirectAno = false;


        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected MiyabiPartnerCardBase(int energy, CardRarity rarity, TargetType target, CardType type = CardType.Skill, bool showInLib=true)
            : base(energy, type, rarity, target, showInLib=true)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 1. 获得护甲
            // 注意：BlockVar 通常会自动关联到 DynamicVars.Block
            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            // 2. 施加招架 (ParryPower)
            if (base.DynamicVars.TryGetValue(ParryVarName, out var parryVar) && parryVar.BaseValue > 0)
            {
                await PowerCmd.Apply<MiyabiParryPower>(base.Owner.Creature, parryVar.BaseValue, base.Owner.Creature, this);
            }

            // 3. 施加滑步 (SlipperyPower)
            if (base.DynamicVars.TryGetValue(SlipperyVarName, out var slipVar) && slipVar.BaseValue > 0)
            {
                // 注意：这里修正了你原代码中 Slippery 误写成 ParryPower 的问题
                await PowerCmd.Apply<SlipperyPower>(base.Owner.Creature, slipVar.BaseValue, base.Owner.Creature, this);
            }
            //施加失衡
            //if(base.DynamicVars.TryGetValue(DazeVarName, out var dazeVar) && dazeVar.BaseValue > 0)
            //{
            //    if (!cardPlay.Target.HasPower<BreakPower>())
            //        await PowerCmd.Apply<DazePower>(base.Owner.Creature, dazeVar.BaseValue, base.Owner.Creature, this);
            //}
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
            {
                await MiyabiCombatService.AddDaze(cardPlay.Target, dazeVar,base.Owner.Creature);
            }

            //属性积蓄与异常
            //if ((base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out var anoVar) && anoVar.BaseValue > 0) || isDirectAno)
            //{
            //    int chkAno = cardPlay.Target.GetPowerAmount<AnomalyBuildupPower>() + anoVar.IntValue;
            //    int trigger = MiyabiCombatService.GetAnoTrigger();

            //    if ((isDirectAno || chkAno >= trigger +1) && cardPlay.Target.HasPower<AttributeAnomalyPower>())
            //    {
            //        //触发紊乱
            //        await PowerCmd.Remove<AttributeAnomalyPower>(cardPlay.Target);
            //        await PowerCmd.Apply<DisorderPower>(cardPlay.Target,1,base.Owner.Creature, this);
            //    }
            //    else if(!cardPlay.Target.HasPower<AttributeAnomalyPower>() 
            //        && ((!isDirectAno && chkAno <= trigger) || isDirectAno))
            //    {
            //        //触发异常
            //        await PowerCmd.Apply<AttributeAnomalyPower>(cardPlay.Target, 1, base.Owner.Creature, this);
            //        if(!isDirectAno)
            //            await PowerCmd.Apply<AnomalyBuildupPower>(cardPlay.Target, -trigger, base.Owner.Creature, this);
            //    }
            //    else if (chkAno <= trigger && !isDirectAno)
            //    { 
            //        //仅添加
            //        await PowerCmd.Apply<AnomalyBuildupPower>(cardPlay.Target, anoVar.BaseValue, base.Owner.Creature, this); 
            //    }
            //}
            if ((base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out var anoVar) && anoVar.BaseValue > 0) || isDirectAno)
            {
                var target = cardPlay.Target;
                int trigger = MiyabiCombatService.GetAnoTrigger();
                bool hasAnomaly = target.HasPower<AttributeAnomalyPower>();
                int chkAno = anoVar.IntValue;
                if (target.HasPower<AnomalyBuildupPower>())
                    chkAno += target.GetPowerAmount<AnomalyBuildupPower>();

                // 情况 A：已经有异常状态了
                if (hasAnomaly)
                {
                    if (isDirectAno || chkAno >= trigger + 1) // 满溢则紊乱
                    {
                        await MiyabiCombatService.DisorderApply(target, base.Owner.Creature, choiceContext);
                    }
                    else // 未满则继续堆积蓄
                    {
                        await PowerCmd.Apply<AnomalyBuildupPower>(target, anoVar.BaseValue, base.Owner.Creature, this);
                    }
                }
                // 情况 B：还没有异常状态
                else
                {
                    if (isDirectAno || chkAno >= trigger + 1) // 满溢则触发异常
                    {
                        await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, base.Owner.Creature, this);
                        if (!isDirectAno) // 扣除触发掉的积蓄
                            await PowerCmd.Apply<AnomalyBuildupPower>(target, -trigger, base.Owner.Creature, this);
                    }
                    else // 未满则仅仅添加积蓄
                    {
                        await PowerCmd.Apply<AnomalyBuildupPower>(target, anoVar.BaseValue, base.Owner.Creature, this);
                    }
                }
            }

            if (base.DynamicVars.Damage.BaseValue > 0)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
            }
        }


        public virtual int CheckSupportCost(int a)
        {
            if(!base.Owner.Creature.HasPower<SupportPointPower>()) return 0;
            return base.Owner.Creature.GetPower<SupportPointPower>().CanUsePoint(a);
            //{
            //    return false;
            //}
            //return true;
        }

        public virtual async Task CostSupporPoint(int amount) 
        {
            if (CheckSupportCost(amount) == 0) return;
            if (CheckSupportCost(amount) == 1)
                await PowerCmd.Apply<SupportPointPower>(base.Owner.Creature,-amount,null,null);
            if (CheckSupportCost(amount) == 2) return;
        }
    }
}
