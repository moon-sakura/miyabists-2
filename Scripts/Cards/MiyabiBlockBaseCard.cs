using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Cards
{
    /// <summary>
    /// 雅组通用防御基类：处理 护甲 + 招架 (Parry) + 滑步 (Slippery) 逻辑
    /// </summary>
    internal abstract class MiyabiBlockCardBase : MiyabiCardBase
    {
        protected const string ParryVarName = "PARRY_POWER";
        protected const string SlipperyVarName = "SLIPPERY_POWER";


        protected MiyabiBlockCardBase(int energy, CardRarity rarity, bool showInLib)
            : base(energy, CardType.Skill, rarity, TargetType.Self, showInLib=true)
        {
        }

        // 告知系统这张卡涉及护甲，以便 UI 显示护甲图标
        public override bool GainsBlock => true;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (DynamicVars.Block.BaseValue > 0 )
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
        }
    }
}