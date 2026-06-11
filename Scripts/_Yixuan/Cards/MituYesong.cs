using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Cards
{
    [RegisterCard(typeof(YixuanCardPool))]
    internal class MituYesong : YixuanPartnerCardBase
    {
        public MituYesong() : base(2, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override string ArtPath => "res://images/_YiXuan/cards/mituYesong.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Turns", 4),
            new DynamicVar(SupportVarName, 1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            MiyabiKeywords.Friends,
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<SupportPointPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 四回合内每次使用非攻击卡后获得2点活力
            var power = await PowerCmd.Apply<MituYesongPower>(choiceContext, Owner.Creature, DynamicVars["Turns"].IntValue, Owner.Creature, this);

            // 支援点数1：恢复所有角色6点生命
            await SupportPointFunc(choiceContext, DynamicVars[SupportVarName].IntValue, async () =>
            {
                foreach (var player in Owner.Creature.CombatState.Players)
                {
                    if (player.Creature.IsAlive)
                        await CreatureCmd.Heal(player.Creature, 6);
                }
            });
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
