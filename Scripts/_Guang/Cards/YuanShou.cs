using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Cards
{
    /// <summary>
    /// 援守 - 罕见攻击卡
    /// 造成7点伤害，失衡5。
    /// 附加：本回合已使用过伙伴卡，则将1张随机普通牌加入你的手牌。
    /// 升级：10点伤害，加入2张卡。
    /// </summary>
    [RegisterCard(typeof(YeshunGuangCardPool))]
    internal class YuanShou : GuangCardBase
    {
        public YuanShou() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        //protected override string ArtPath => "res://images/_Guang/cards/yuanShou.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(7, ValueProp.Move),
            new DynamicVar(DazeVarName, 5),
            new CardsVar(1),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DazePower>(),
            HoverTipFactory.FromPower<BreakPower>(),
            HoverTipFactory.FromPower<DazeVulnPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await MiyabiCombatService.AddDaze(choiceContext, cardPlay.Target, DynamicVars[DazeVarName], base.Owner.Creature);

            // 附加：本回合使用过伙伴卡，则加入随机普通牌
            bool usedFriendThisTurn = CombatManager.Instance.History.CardPlaysFinished
                .Any((CardPlayFinishedEntry e)
                    => e.CardPlay.Card.Owner == base.Owner
                    && e.CardPlay.Card.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                    && e.HappenedThisTurn(base.CombatState));

            if (!usedFriendThisTurn) return;

            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                CardModel card = CardFactory.GetDistinctForCombat(Owner, from c in Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                                                                       where c.Rarity == CardRarity.Common
                                                                       select c, 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (card == null) break;

                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
