using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Service;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Relics
{
    [RegisterRelic(typeof(MiyabiRelicPool))]
    internal class SectionSixRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/relics/section_six.png";
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

        private static readonly string[] MiyabiHurtedVoices = { "hurted_1", "hurted_2"};

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target != base.Owner.Creature || result.WasFullyBlocked || result.UnblockedDamage <= 1) { return; }

            // 随机选一句
            int idx = (int)(GD.Randi() % MiyabiHurtedVoices.Length);

            // 它会自动处理加载、播放、音量转换和自动销毁
            MiyabiAudioService.Play(MiyabiHurtedVoices[idx]);

            GD.Print("[MiyabiMod] 播放受伤语音: " + MiyabiHurtedVoices[idx]);
        }


    }
}
