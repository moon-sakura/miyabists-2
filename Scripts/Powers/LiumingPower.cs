using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Powers
{
    /// <summary>
    /// 流明：每回合结束时受到5%属性异常伤害，与异常同时存在时触发耀变，不可叠加
    /// </summary>
    internal class LiumingPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/powers/liuming.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            //HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            //HoverTipFactory.FromPower<YaobianPower>(),
        ];

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;

            // 受到5%属性异常伤害
            await MiyabiCombatService.DealAnoDamage(choiceContext, null, Owner, 5);
        }

        private bool _isConverting = false;
        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power.Owner != base.Owner)
                return;

            if (_isConverting) return;

            if (power is AttributeAnomalyPower || power is LiumingPower)
            {
                _isConverting = true;
                try
                {
                    await PowerCmd.Decrement(this);
                    await PowerCmd.Remove(Owner.GetPower<AttributeAnomalyPower>());
                    await MiyabiCombatService.DealAnoDamage(choiceContext, null, Owner, 5);
                    await PowerCmd.Apply<YaobianPower>(choiceContext, Owner, 1m, null, null);
                }
                finally
                {
                    _isConverting = false;
                }
            }
        }
    }
}
