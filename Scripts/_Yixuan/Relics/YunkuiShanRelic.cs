using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
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

namespace Miyabists2.Scripts._Yixuan.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class YunkuiShanRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/_YiXuan/relics/yunkuishan.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;
        protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<SupportPointPower>()
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<SupportPointPower>(choiceContext, base.Owner.Creature, 7, null, null);
            }
        }

        private static readonly string[] YixuanHurtedVoices = { "yixuan_hurt_1", "yixuan_hurt_2", "yixuan_hurt_3" };

        private static readonly string[] YixuanHurtedHeavyVoices = { "yixuan_hurt_heavy_1", "yixuan_hurt_heavy_2", "yixuan_hurt_heavy_3" };

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != base.Owner.Creature || result.WasFullyBlocked || dealer == Owner.Creature) { return; }

            // 从受伤语音池随机播放
            if(result.UnblockedDamage >= 10)
                MiyabiAudioPlay.Random(YixuanHurtedHeavyVoices);
            else
                MiyabiAudioPlay.Random(YixuanHurtedVoices);
        }
    }
}
