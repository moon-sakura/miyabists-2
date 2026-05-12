using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    internal class BlessingOfmoon : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/moonBlessing.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => [MiyabiKeywords.OtherWorldFriends];

        public BlessingOfmoon()
            : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Bless", 25)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AttributeAnomalyPower>(),
            HoverTipFactory.FromPower<DisorderPower>(),
        ];

        private static readonly string[] MiyabiBlessingMoonVoices = { "card_BlessingMoon_1", "card_BlessingMoon_2", "card_BlessingMoon_3" };

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (base.DynamicVars.TryGetValue("Bless", out DynamicVar b))
                await PowerCmd.Apply<BlessingMoonPower>(choiceContext, base.Owner.Creature, b.BaseValue, Owner.Creature, this);

            // 随机选一句
            int idx = (int)(GD.Randi() % MiyabiBlessingMoonVoices.Length);

            // 它会自动处理加载、播放、音量转换和自动销毁
            MiyabiAudioService.Play(MiyabiBlessingMoonVoices[idx],1.2f);
        }

        protected override void OnUpgrade()
        {
            if (base.DynamicVars.TryGetValue("Bless", out DynamicVar b)) b.UpgradeValueBy(15);
        }

    }
}
