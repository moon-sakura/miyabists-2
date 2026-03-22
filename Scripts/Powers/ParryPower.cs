using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Cards;

namespace Miyabists2.Scripts.Powers
{
    internal class MiyabiParryPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/Frost.png";
        public string BigBetaIconPath => "res://images/powers/Frost.png";
        public override string CustomPackedIconPath => "res://images/powers/Frost.png";
        public override string CustomBigIconPath => "res://images/powers/Frost.png";


        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (result.BlockedDamage > 0 && base.Amount > 0)
            {
                // 2. 逻辑触发：增加一张《花辞》到手牌
                // 注意：这里使用你之前定义的卡牌 ID "HuaCi"
                CardModel reward1 = base.Owner.CombatState.CreateCard<HuaCi>(base.Owner.Player);
                await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, addedByPlayer: true, CardPilePosition.Random);

                // 3. 消耗一层招架层数
                await PowerCmd.TickDownDuration(this);
            }
        }

    }
}
