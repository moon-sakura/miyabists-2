using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Cards.NoneShow;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class MohenHuaxing : YixuanBlockCardBase
    {
        public MohenHuaxing() : base(2, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/mohenHuanxing.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(6, ValueProp.Move),
            new DynamicVar(ThornsVarName, 3),
            new DynamicVar(VigorVarName, 3),
            new DynamicVar(ShannengVarName, 10),
            //new DynamicVar("ExtraVigor", 5),
        ];

        public override bool GainsBlock => false;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 覆甲
            await PowerCmd.Apply<PlatingPower>(choiceContext, Owner.Creature, DynamicVars.Block.IntValue, Owner.Creature, this);

            // 荆棘或活力二选一
            await MiyabiCombatService.ChooseResYi(choiceContext, base.Owner, new Dictionary<Type, (int, Func<PlayerChoiceContext, Task>)>
            {
                { typeof(ThornsChoice), (DynamicVars[ThornsVarName].IntValue, async ctx => {
                    await PowerCmd.Apply<ThornsPower>(ctx, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);
                })},
                { typeof(VigorChoice), (DynamicVars[VigorVarName].IntValue, async ctx => {
                    await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, DynamicVars[VigorVarName].IntValue, Owner.Creature, this);
                })},
            });

            // 恢复闪能
            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner.Creature, DynamicVars[ShannengVarName].IntValue, Owner.Creature, this);

            // 如果敌人是攻击意图：额外活力 + 下一张玄墨卡0费
            bool enemyAttacking = Owner.Creature.CombatState.Enemies.Any(e => e.IsAlive && e.Monster.IntendsToAttack);
            if (enemyAttacking)
            {
                await PowerCmd.Apply<MohenhxPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            DynamicVars[ThornsVarName].UpgradeValueBy(2);
            DynamicVars[VigorVarName].UpgradeValueBy(2);
        }
    }
}
