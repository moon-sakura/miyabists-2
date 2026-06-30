using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Cards;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    internal abstract class YixuanPartnerCardBase : YixuanCardBase
    {
        protected const string SupportVarName = "SUPPORT_POINT_POWER";

        protected override bool ShouldGlowGoldInternal
        {
            get
            {
                if (SupportFree > 0)
                {
                    return true;
                }
                if (DynamicVars.TryGetValue(SupportVarName, out DynamicVar s))
                {
                    return Owner.Creature.GetPowerAmount<SupportPointPower>() >= s.IntValue;
                }
                return false;
            }
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.Friends];

        protected YixuanPartnerCardBase(int energy, CardRarity rarity, TargetType target, CardType type = CardType.Skill, bool showInLib = true)
            : base(energy, type, rarity, target, showInLib)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //// 施加失衡
            //if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
            //{
            //    await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, dazeVar, base.Owner.Creature);
            //}

            //// 属性积蓄与异常
            //if (base.DynamicVars.TryGetValue(AnomalyBuildupVarName, out var anoVar) && anoVar.BaseValue > 0)
            //{
            //    await MiyabiCombatService.AddAnoBuildup(cardPlay.Target, anoVar.IntValue, base.Owner.Creature, this, choiceContext);
            //}
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if(cardSource == this)
            {
                if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar dazeVar) && dazeVar.BaseValue > 0)
                {
                    await MiyabiCombatService.AddDaze(choiceContext, target, dazeVar, base.Owner.Creature);
                }
            }
        }

        /// <summary>支援点数条件触发：检查支援点数 → 执行动作 → 消耗支援点数</summary>
        protected async Task SupportPointFunc(PlayerChoiceContext choiceContext, int supportT, Func<Task> FriendFunc, bool isForceTrigger = false, bool isFreeThis = false)
        {
            if (CheckSupportCost(supportT) != 0 || isForceTrigger)
            {
                await FriendFunc();

                if (SupportFree >= 1)
                {
                    SupportFree -= 1;
                    return;
                }

                if (!isFreeThis)
                    await CostSupporPoint(supportT, choiceContext);
            }
        }

        public virtual int CheckSupportCost(int a)
        {
            if (!base.Owner.Creature.HasPower<SupportPointPower>()) return 0;
            if (SupportFree >= 1) return 3;
            return base.Owner.Creature.GetPower<SupportPointPower>().CanUsePoint(a);
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
