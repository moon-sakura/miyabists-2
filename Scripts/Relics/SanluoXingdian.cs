using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class SanluoXingdianRelic : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override string PackedIconPath => "res://images/relics/sanluoXingdian.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        private int _counter;

        // 显示在遗物图标上的数字
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

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            //HoverTipFactory.FromCard<JixianShiyu>(),
            //HoverTipFactory.FromPower<FrostFallPower>(),
            //HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                if(Counter >= 7)
                {
                    Flash();
                    if (Counter >= 15)
                    {
                        CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<EJiZhan>(base.Owner.Creature.Player);
                        await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);

                    }
                    else
                    {
                        CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<MingCanXue>(base.Owner.Creature.Player);
                        await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner, CardPilePosition.Random);
                    }
                }
            }
        }

        public override Task AfterCombatVictory(CombatRoom room)
        {
            if(room.RoomType == RoomType.Monster)
            {
                Counter += 1; // 增加计数器
            }
            else if(room.RoomType == RoomType.Elite)
            {
                Counter += 2; // 增加计数器
            }
            else if(room.RoomType == RoomType.Boss)
            {
                Counter += 3; // 增加计数器
            }
            return base.AfterCombatVictory(room);
        }
    }
}
