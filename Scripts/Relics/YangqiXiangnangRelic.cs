using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class YangqiXiangnangRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override string PackedIconPath => "res://images/relics/yangqiXiangnang.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        // --- 计数器逻辑 (如不需要可删除) ---
        private int _counter;
        public override bool ShowCounter => true;
        public override int DisplayAmount => Counter;

        [SavedProperty]
        public int Counter
        {
            get => _counter;
            private set
            {
                AssertMutable();
                _counter = value;
                InvokeDisplayAmountChanged();
            }
        }
        // ------------------------------

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
            // HoverTipFactory.FromCard<MyCard>(),
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAX",60),
            new DynamicVar("ADD",10),
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) return;

            if(base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                // 查找实例
                var myRelic = Owner.Relics.OfType<SwordNotailRelic>().FirstOrDefault();
                var myRelic2 = Owner.Relics.OfType<NoTailFullRelic>().FirstOrDefault();

                if (myRelic != null)
                {
                    myRelic?.SetMax(DynamicVars["MAX"].IntValue);
                }

                if (myRelic2 != null)
                {
                    myRelic2?.SetMax(DynamicVars["MAX"].IntValue);
                }
            }

            Counter++;

            if(Counter >= 3)
            {
                Flash();
                await MiyabiCombatService.AddDecible(base.Owner, DynamicVars["ADD"].IntValue);
                Counter = 0;
            }
        }

        public override Task AfterCombatEnd(CombatRoom room)
        {
            // 查找实例
            var myRelic = Owner.Relics.OfType<SwordNotailRelic>().FirstOrDefault();
            var myRelic2 = Owner.Relics.OfType<NoTailFullRelic>().FirstOrDefault();

            if (myRelic != null)
            {
                myRelic?.ResetMax();
            }

            if (myRelic2 != null)
            {
                myRelic2?.ResetMax();
            }
            return base.AfterCombatEnd(room);
        }
    }
}
