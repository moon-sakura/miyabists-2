using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class LiuyunYingshen : YixuanAtkCardBase
    {
        public LiuyunYingshen() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 5),
            new DynamicVar(ThornsVarName, 2),
            new DynamicVar("SelfDamage", 3),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ThornsPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 先对自己造成伤害（视为随机敌人造成）
            var enemy = Owner.Creature.CombatState.Enemies.TakeRandom(1, Owner.Creature.CombatState.RunState.Rng.Shuffle).FirstOrDefault();
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars["SelfDamage"].BaseValue, ValueProp.Unpowered, enemy);

            // 再执行基础攻击逻辑（玄墨伤害 + 失衡值 + 闪能）
            await base.OnPlay(choiceContext, cardPlay);

            // 获得荆棘
            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars[ThornsVarName].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(3);
            DynamicVars[ThornsVarName].UpgradeValueBy(1);
        }
    }
}
