using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class NingyunShu : YixuanAtkCardBase
    {
        public NingyunShu() : base(1, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/ningyunshu.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(5, ValueProp.Unblockable | ValueProp.Move),
            new DynamicVar(DazeVarName, 3),
            new DynamicVar(ShufaVarName, 5),
            new DynamicVar(ShannengVarName, 30),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Xuanmo,
            //CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ShufaZhi>(),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override bool ShouldGlowGoldInternal => CheckShannengCost(DynamicVars[ShannengVarName].IntValue) > 0;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);

            // 施加术法值
            await PowerCmd.Apply<ShufaZhi>(choiceContext, cardPlay.Target, DynamicVars[ShufaVarName].IntValue, Owner.Creature, this);

            // 闪能10：添加一张消耗虚无的复制到手卡，本回合免费打出
            await ShannengFunc(choiceContext, DynamicVars[ShannengVarName].IntValue, async () =>
            {
                CardModel copy = this.CreateClone();
                copy.AddKeyword(CardKeyword.Exhaust);
                copy.AddKeyword(CardKeyword.Ethereal);
                copy.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
            });
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
        }
    }
}
