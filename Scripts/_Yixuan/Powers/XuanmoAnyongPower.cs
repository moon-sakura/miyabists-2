using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class XuanmoAnyongPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/powers/xuanmoAnyong.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends))
            {
                await PowerCmd.Apply<ShannengPower>(choiceContext, Owner, Amount*2, Owner, null);
                var target = cardPlay.Target;
                if(target == null || target.IsPlayer)
                {
                    target = Owner.Player.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
                }
                await CreatureCmd.Damage(choiceContext, target, Amount * 3, ValueProp.Unblockable | ValueProp.Unpowered, (Creature)null);
            }
        }
    }
}
