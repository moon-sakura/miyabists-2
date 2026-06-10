using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Powers
{
    internal class QingxiaoTongyouPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override int DisplayAmount => Amount;

        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public string BigIconPath => "res://images/_YiXuan/char/common.png";
        public string BigBetaIconPath => BigIconPath;
        public override string CustomIconPath => BigIconPath;
        public override string CustomBigIconPath => BigIconPath;

        private int _vigorAmount = 0;
        private int _thornsAmount = 0;
        private int _shannengAmount = 0;

        public void SetAmounts(int vigor, int thorns, int shanneng)
        {
            _vigorAmount += vigor;
            _thornsAmount += thorns;
            _shannengAmount += shanneng;
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            await PowerCmd.Apply<VigorPower>(choiceContext, Owner, _vigorAmount, Owner, null);
            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner, _thornsAmount, Owner, null);
            await PowerCmd.Apply<ShannengPower>(choiceContext, Owner, _shannengAmount, Owner, null);
        }
    }
}
