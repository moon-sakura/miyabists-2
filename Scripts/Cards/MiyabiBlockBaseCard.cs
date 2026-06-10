using STS2RitsuLib.Interop.AutoRegistration;
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
    [RegisterCard(typeof(MiyabiCardPool))]
    /// <summary>
    /// 闆呯粍閫氱敤闃插尽鍩虹被锛氬鐞?鎶ょ敳 + 鎷涙灦 (Parry) + 婊戞 (Slippery) 閫昏緫
    /// </summary>
    internal abstract class MiyabiBlockCardBase : MiyabiCardBase
    {
        protected MiyabiBlockCardBase(int energy, CardRarity rarity, bool showInLib)
            : base(energy, CardType.Skill, rarity, TargetType.Self, showInLib=true)
        {
        }

        // 鍛婄煡绯荤粺杩欏紶鍗℃秹鍙婃姢鐢诧紝浠ヤ究 UI 鏄剧ず鎶ょ敳鍥炬爣
        public override bool GainsBlock => true;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (GainsBlock)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            // 2. 鏂藉姞鎷涙灦 (ParryPower)
            if (base.DynamicVars.TryGetValue(ParryVarName, out var parryVar) && parryVar.BaseValue > 0)
            {
                await PowerCmd.Apply<MiyabiParryPower>(choiceContext,base.Owner.Creature, parryVar.BaseValue, base.Owner.Creature, this);
            }

            // 3. 鏂藉姞婊戞 (SlipperyPower)
            if (base.DynamicVars.TryGetValue(SlipperyVarName, out var slipVar) && slipVar.BaseValue > 0)
            {
                await PowerCmd.Apply<SlipperyPower>(choiceContext,base.Owner.Creature, slipVar.BaseValue, base.Owner.Creature, this);
            }
        }
    }
}