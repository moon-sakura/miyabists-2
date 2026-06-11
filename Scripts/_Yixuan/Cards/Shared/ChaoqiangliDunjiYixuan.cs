using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class ChaoqiangliDunjiYixuan : ChaoqiangliDunji
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(11,ValueProp.Move),
            new DynamicVar("THORNS_POWER", 3),
            new DynamicVar(SupportVarName,3),
            new DynamicVar(ExhaustCountVarName, GetExhaustUses()),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),

        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (DynamicVars.Block.BaseValue > 0)
                await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

            await TryExhaustAfterUse(choiceContext, cardPlay);

            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars["THORNS_POWER"].IntValue, Owner.Creature, this);

            await base.SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () => await FriendFunc(choiceContext, 0));
        }

        protected override async Task FriendFunc(PlayerChoiceContext choiceContext, int parryCount)
        {
            int amount = Owner.Creature.GetPowerAmount<ThornsPower>();
            await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, amount, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner.Creature); ;
        }
    }
}
