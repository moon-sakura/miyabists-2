
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Relics
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class ChaoyueJuejingZhizhiRelic : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override string PackedIconPath => "res://images/relics/chaoyueJuejing.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            // HoverTipFactory.FromCard<MyCard>(),
        ];

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (dealer != base.Owner.Creature)
            {
                return 1m;
            }
            bool isValidMove = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
            if (!isValidMove) return 1m;

            decimal percent = (decimal)base.Owner.Creature.CurrentHp / base.Owner.Creature.MaxHp;
            //GD.Print($"[MiyabiSTS2] 此时生命值比例为: {percent}");
            if (percent >= 0.8m) return 0.5m;
            if (percent <= 0.2m) return 1.5m;
            if (percent <= 0.5m) return 1.2m;
            return 1m;
        }

    }
}
