using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Miyabists2.Scripts.Cards
{
    internal class TestCardFrost : MiyabiCardBase
    {
        public TestCardFrost()
            : base(0, CardType.Attack, CardRarity.None, TargetType.AnyEnemy, false)
        {
        }


        // 通用打出逻辑
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            Creature target = cardPlay.Target;
            ArgumentNullException.ThrowIfNull(target, "cardPlay.Target");
            if (cardPlay.Card != this || target == null || target.IsDead) return;
            int chkFB = target.GetPowerAmount<FrostBuildPower>() + 50;

            // 确保是本卡造成的实际伤害，且目标存活
            if (chkFB <= 100)
            {
                // 如果拥有烈霜词条，按伤害量施加积蓄值
                if (!target.HasPower<FrostPower>())
                {
                    await PowerCmd.Apply<FrostBuildPower>(target, 50, base.Owner.Creature, this);
                }
            }
            //烈霜积蓄值积攒逻辑
            if (chkFB >= 101)
            {
                await PowerCmd.SetAmount<FrostBuildPower>(target, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<FrostPower>(target, 1, base.Owner.Creature, this);
                if (target.HasPower<AttributeAnomalyPower>())
                {
                    await PowerCmd.Remove<AttributeAnomalyPower>(target);
                    await PowerCmd.Apply<DisorderPower>(target, 1, base.Owner.Creature, this);
                }
                else
                {
                    await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, base.Owner.Creature, this);
                }
            }
        }
    }
}
