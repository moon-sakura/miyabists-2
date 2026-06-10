using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    public abstract class YixuanBlockCardBase : YixuanCardBase
    {
        public YixuanBlockCardBase(int baseCost, CardRarity rarity, TargetType target, CardType type = CardType.Skill, bool showInCardLibrary = true)
            : base(baseCost, type, rarity, target, showInCardLibrary)
        {
        }

        public override bool GainsBlock => true;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (GainsBlock)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            }

            if(DynamicVars.TryGetValue(ThornsVarName, out DynamicVar thornsVar) && thornsVar.IntValue > 0)
            {
                await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, thornsVar.IntValue, Owner.Creature, this);
            }

            if(DynamicVars.TryGetValue(VigorVarName, out DynamicVar vigorVar) && vigorVar.IntValue > 0)
            {
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, vigorVar.IntValue, Owner.Creature, this);
            }
        }

        //public override string PortraitPath => $"res://images/cards/fengHua.png";
    }
}
