using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Yixuan.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    /// <summary>
    /// 消灾渡厄能力：每使用10张卡，将一张符法千重·破加入手卡
    /// </summary>
    internal class XiaozaiDuePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/xiaozaiDue.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("CardsPerTrigger", 10),
            new DynamicVar("CardsPlayed", 0),
        ];

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;
            if (cardPlay.Card == null) return;

            DynamicVars["CardsPlayed"].BaseValue += 1;

            if (DynamicVars["CardsPlayed"].IntValue >= DynamicVars["CardsPerTrigger"].IntValue)
            {
                DynamicVars["CardsPlayed"].BaseValue = 0;

                for (int i = 0; i < Amount; i++)
                {
                    CardModel reward = Owner.CombatState.CreateCard<FufaQianchongPo>(Owner.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward, PileType.Hand, Owner.Player, CardPilePosition.Random);
                }
            }
        }
    }
}
