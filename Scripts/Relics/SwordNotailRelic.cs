using BaseLib.Abstracts;
using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;

namespace Miyabists2.Scripts.Relics
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class SwordNotailRelic : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => "res://images/relics/OWmajor.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;
        protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

        //public int LuoShuangCostThisTurn { get; set; } = 0; // 本回合已使用的落霜点数

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<FrostFallPower>(base.Owner.Creature, 4, null, null);
            }

            //LuoShuangCostThisTurn = 0;
            

            // 此时，syncRnd.Next 在所有客户端产生的结果将完全一致
            int result = base.Owner.RunState.Rng.Shuffle.NextInt(1, 4); ;

            
        }
    }
}
