using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class HuaFa : MiyabiBlockCardBase
    {
        //public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public HuaFa() : base(2, CardRarity.Uncommon,true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(14, ValueProp.Move),
            new DynamicVar(ParryVarName, 2),
            new DynamicVar(SlipperyVarName, 0)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(MiyabiKeywords.Friends),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int amount = CombatManager.Instance.History.CardPlaysFinished
                .Count((CardPlayFinishedEntry e) 
                => e.CardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends) 
                && e.CardPlay.Card.Owner == base.Owner 
                && e.HappenedThisTurn(base.CombatState));
            await base.OnPlay(choiceContext, cardPlay);
            if (amount > 0)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, 6m, ValueProp.Unpowered, cardPlay);
                await PowerCmd.Apply<MiyabiParryPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            }

        }

        protected override void OnUpgrade()
        {
            // 升级增加 2 点护甲
            DynamicVars.Block.UpgradeValueBy(4);

            // 如果需要升级 Parry 或 Slippery，可以在此添加逻辑
            // if (base.DynamicVars.TryGetValue(ParryVarName, out var v)) v.UpgradeValueBy(1);
        }
    }
}
