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
    /// 决裁 - 能力。
    /// 回合开始时，随机将 1 张《遗忘》或《失感》加入手卡。
    /// 被移除时，变更为《明心镜·决裁》。
    /// </summary>
    internal class JueCaiPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

        public string BigIconPath => "res://images/_Guang/powers/jueCai.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner.Player) return;

            Player owner = Owner.Player;
            if (owner?.Creature?.CombatState == null) return;

            // 随机在《遗忘》与《失感》之间二选一
            bool useYiWang = owner.RunState.Rng.CombatCardGeneration.NextItem(new[] { true, false });

            if (useYiWang)
            {
                await CardPileCmd.AddToCombatAndPreview<YiWang>(Owner, PileType.Discard, 1, Owner.Player);
            }
            else
            {
                await CardPileCmd.AddToCombatAndPreview<ShiGan>(Owner, PileType.Discard, 1, Owner.Player);
            }

            //CardModel? curse = useYiWang
            //    ? owner.Creature.CombatState.CreateCard<YiWang>(owner)
            //    : owner.Creature.CombatState.CreateCard<ShiGan>(owner);

            //if (curse == null) return;

            //await CardPileCmd.AddGeneratedCardToCombat(curse, PileType.Hand, owner);
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            // 战斗结束时的清场不算"被移除"
            if (!CombatManager.Instance.IsInProgress) return;
            if (oldOwner == null) return;

            // 变更为《明心镜·决裁》
            await PowerCmd.Apply<MingxinJueCaiPower>(new ThrowingPlayerChoiceContext(), oldOwner, 1, oldOwner, null);
        }
    }
}
