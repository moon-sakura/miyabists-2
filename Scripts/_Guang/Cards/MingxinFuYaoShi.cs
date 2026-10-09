using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Miyabists2.Scripts._Guang.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 明心镜·扶摇势（原卡：扶摇势）
    /// 获得1层翱翔。
    /// 附加：消耗1点青溟剑势，将一张带有消耗的《明心镜·斩流光》加入手卡，本回合免费打出。
    /// 被消耗时获得12点活力。
    /// 升级：0费。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class MingxinFuYaoShi : GuangMingxinjingCardBase
    {
        public MingxinFuYaoShi() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Soar", 1),
            new DynamicVar(JianshiVarName, 1),
            new DynamicVar("VigorOnExhaust", 12),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<QingmingJianshiPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromCard<MingxinZhanLiuGuang>(),
            // TODO: 翱翔能力写好后取消注释
            // HoverTipFactory.FromPower<AoxiangPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // ========== 待实现 ==========
            // TODO: 获得 DynamicVars["Soar"] 层翱翔

            // 附加：消耗1点青溟剑势 -> 将一张带有消耗的《明心镜·斩流光》加入手卡，本回合免费打出
            await JianshiFunc(choiceContext, DynamicVars[JianshiVarName].IntValue, async () =>
            {
                CardModel card = Owner.Creature.CombatState.CreateCard<MingxinZhanLiuGuang>(Owner);
                card.AddKeyword(CardKeyword.Exhaust);
                card.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            });
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 升级后0费
        }

        // 被消耗时获得12点活力
        protected override async Task ExhaustEffect(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature,
                DynamicVars["VigorOnExhaust"].IntValue, Owner.Creature, this);
        }
    }
}
