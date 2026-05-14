using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class EousBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/eous_bangboo.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, Creature self, MinionSummonOptions options) // 注意使用 self 而非 this
        {
            //await base.OnSummon(choiceContext, owner, self, options); // 先调用基类的 OnSummon 来设置血量等基础属性

            //base.IsHealthBarVisible = true;
            //if (options.PrimaryStatAmount is decimal buffer && buffer > 0m)
                //await PowerCmd.Apply<EousAct>(new ThrowingPlayerChoiceContext(), self, buffer, owner.Creature, options.Source);
        }
    }

    internal class EousAct : MiyabiBangbooActBase
    {
        //protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        //{
        //    var relic = MiyabiFuncBase.GetRelic<EousBangbooRelic>(Owner.PetOwner);

        //    if (relic == null || relic.Counter <= 0)
        //        return;

        //    await PowerCmd.Apply<BufferPower>(choiceContext, Owner.PetOwner.Creature, 1m, Owner, null);

        //    relic.DecreaseCounter();
        //}
    }
}
