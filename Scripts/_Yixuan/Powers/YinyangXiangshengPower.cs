using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    /// <summary>
    /// 阴阳相生：下一次获得格挡时获得对应数值的活力，或下一次造成伤害时获得对应数值的荆棘
    /// </summary>
    internal class YinyangXiangshengPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/yiyangXiangsheng.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private int _previousBlock = 0;

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _previousBlock = (int)Owner.Block;
        }

        public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
        {
            if (dealer != Owner) return;
            if (cardSource == null) return;

            int thornsAmount = (int)result.TotalDamage;
            if (thornsAmount <= 0) return;

            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner, thornsAmount, Owner, cardSource);
            //await PowerCmd.Remove<YinyangXiangshengPower>(Owner);
            await PowerCmd.Decrement(this);
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;

            // 检查是否已被AfterDamageGiven移除
            if (!Owner.HasPower<YinyangXiangshengPower>()) return;

            int currentBlock = (int)Owner.Block;
            if (currentBlock > _previousBlock)
            {
                int gained = currentBlock - _previousBlock;
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner, gained, Owner, null);
                //await PowerCmd.Remove<YinyangXiangshengPower>(Owner);
                await PowerCmd.Decrement(this);
                return;
            }
            _previousBlock = currentBlock;
        }
    }
}
