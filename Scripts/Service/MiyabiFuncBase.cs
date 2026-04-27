using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using System.Drawing;

namespace Miyabists2.Scripts.Service
{
    internal class MiyabiFuncBase
    {
        //设置能力到指定值
        public static async Task SetPowerAmount(PlayerChoiceContext context,PowerModel power,int powerAmount, Creature? applier, CardModel? cardSource, bool silent = false) 
        {
            if (power == null) return;

            int currentAmount = power.Amount;
            await PowerCmd.ModifyAmount(context, power, powerAmount - currentAmount, applier, cardSource);
        }

        public static int RadomInt(int Min , int exMax, Player player)
        {
            int result = player.RunState.Rng.Shuffle.NextInt(Min, exMax);
            return result;
        }

        //通用PlayerChoiceContext
        //public static PlayerChoiceContext choiceContext = new HookPlayerChoiceContext(Owner, Owner.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any);
    }
}
