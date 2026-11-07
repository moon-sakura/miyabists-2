using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts._Guang.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 英雄！ - 本回合每使用 1 张卡，随机将 1 张《遗忘》或《失感》加入弃牌堆。
    /// 回合结束时移除。
    /// </summary>
    internal class YingXiongPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/yingXiong.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay?.Card?.Owner != Owner.Player) return;

            Player? owner = Owner.Player;
            if (owner?.Creature?.CombatState == null) return;

            // 随机在《遗忘》与《失感》之间二选一，放入弃牌堆
            bool useYiWang = owner.RunState.Rng.CombatCardGeneration.NextItem(new[] { true, false });

            //CardModel? curse = useYiWang
            //    ? owner.Creature.CombatState.CreateCard<YiWang>(owner)
            //    : owner.Creature.CombatState.CreateCard<ShiGan>(owner);
            if (useYiWang)
            {
                await CardPileCmd.AddToCombatAndPreview<YiWang>(Owner, PileType.Discard, 1, Owner.Player);
            }
            else
            {
                await CardPileCmd.AddToCombatAndPreview<ShiGan>(Owner, PileType.Discard, 1, Owner.Player);
            }

            //if (curse == null) return;

            //await CardPileCmd.AddGeneratedCardToCombat(curse, PileType.Discard, owner);
        }

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }
    }
}
