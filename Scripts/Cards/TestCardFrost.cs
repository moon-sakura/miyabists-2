using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Relics;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Miyabists2.Scripts.Cards
{
    [Pool(typeof(StatusCardPool))]
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

            MiyabiCombatService.SetFrostTriggerMultiply(base.Owner.Creature);
            int trigger = MiyabiCombatService.GetFrostTrigger();

            //int chkFB = target.GetPowerAmount<FrostBuildPower>() + result.TotalDamage;

            // 确保是本卡造成的实际伤害，且目标存活
            if (chkFB <= trigger && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
            {
                // 如果拥有烈霜词条，按伤害量施加积蓄值
                if (this.CanonicalKeywords.Contains(MiyabiKeywords.LieShuang))
                {
                    await PowerCmd.Apply<FrostBuildPower>(target, 50, base.Owner.Creature, this);
                }
            }
            //烈霜积蓄值积攒逻辑
            if (chkFB >= trigger + 1 && (!target.HasPower<FrostPower>() || MiyabiCombatService.GetCanAddWhenFire()))
            {
                //await MiyabiCombatService.FrostApply(target,base.Owner.Creature,choiceContext);
                await PowerCmd.SetAmount<FrostBuildPower>(target, 1, base.Owner.Creature, this);
                await PowerCmd.Apply<FrostPower>(target, 1, base.Owner.Creature, this);

                //int fireAmount = target.GetPowerAmount<FrostFirePower>();
                //await CreatureCmd.Damage(null, target, 10m, ValueProp.Unpowered, dealer);


                if (target.HasPower<AttributeAnomalyPower>())
                {
                    await MiyabiCombatService.DisorderApply(target, base.Owner.Creature, choiceContext);
                }
                else
                {
                    await PowerCmd.Apply<AttributeAnomalyPower>(target, 1, base.Owner.Creature, this);
                }
            }
        }
    }
}
