using STS2RitsuLib.Interop.AutoRegistration;
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
    //[RegisterCard(typeof(MiyabiCardPool))]
    internal abstract class MiyabiPartnerCardBase : MiyabiCardBase
    {
        // 伙伴卡通常消耗的支援点数变量名
        protected int _supportCost = 0; // 默认需要 0 点支援点数

        protected const string SupportVarName = "SUPPORT_POINT_POWER";

        //protected const bool isDirectAno = false;

        protected override bool ShouldGlowGoldInternal
        {
            get
            {
                if(DynamicVars.TryGetValue(SupportVarName, out DynamicVar s))
                {
                    return Owner.Creature.GetPowerAmount<SupportPointPower>() >= s.IntValue;
                }
                return false;
            }
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected MiyabiPartnerCardBase(int energy, CardRarity rarity, TargetType target, CardType type = CardType.Skill, bool showInLib = true)
            : base(energy, type, rarity, target, showInLib = true)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            // 2. 施加招架 (ParryPower)
            if (base.DynamicVars.TryGetValue(ParryVarName, out var parryVar) && parryVar.BaseValue > 0)
            {
                await PowerCmd.Apply<MiyabiParryPower>(choiceContext, base.Owner.Creature, parryVar.BaseValue, base.Owner.Creature, this);
            }

            // 3. 施加滑步 (SlipperyPower)
            if (base.DynamicVars.TryGetValue(SlipperyVarName, out var slipVar) && slipVar.BaseValue > 0)
            {
                await PowerCmd.Apply<SlipperyPower>(choiceContext, base.Owner.Creature, slipVar.BaseValue, base.Owner.Creature, this);
            }
            //施加失衡
            //if(base.DynamicVars.TryGetValue(DazeVarName, out var dazeVar) && dazeVar.BaseValue > 0)
            //{
            //    if (!cardPlay.Target.HasPower<BreakPower>())
            //        await PowerCmd.Apply<DazePower>(base.Owner.Creature, dazeVar.BaseValue, base.Owner.Creature, this);
            //}
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
            {
                await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, dazeVar, base.Owner.Creature);
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
            if ((base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out var anoVar) && anoVar.BaseValue > 0))
            {
                await MiyabiCombatService.AddAnoBuildup(cardPlay.Target, anoVar.IntValue, base.Owner.Creature, this, choiceContext);
            }


        }

        //以下卡牌没有使用该函数
        //蛇吻
        //甜蜜惊吓
        //终末裁决
        //醉花月云转
        protected async Task SupportPointFunc(PlayerChoiceContext choiceContext, int supportT, Func<Task> FriendFunc, bool isForceTrigger = false, bool isFreeThis = false)
        {
            if (CheckSupportCost(supportT) != 0 || isForceTrigger)
            {
                await FriendFunc();

                if (!isFreeThis)
                    await CostSupporPoint(supportT, choiceContext);
            }
        }


        public virtual int CheckSupportCost(int a)
        {
            if (!base.Owner.Creature.HasPower<SupportPointPower>()) return 0;
            return base.Owner.Creature.GetPower<SupportPointPower>().CanUsePoint(a);
            //{
            //    return false;
            //}
            //return true;
        }

        public virtual async Task CostSupporPoint(int amount, PlayerChoiceContext choiceContext)
        {
            if (CheckSupportCost(amount) == 0) return;
            if (CheckSupportCost(amount) == 1)
                await PowerCmd.Apply<SupportPointPower>(choiceContext, base.Owner.Creature, -amount, null, null);
            if (CheckSupportCost(amount) == 2) return;
        }
    }
}
