using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    internal class MiyabiBangbooService
    {
        //public static List<MinionModel> BangbooList { get; set; }

        //public static void ResignAllBangboo()
        //{
        //    ModelDb.Monster<EousBangboo>();
        //    ModelDb.Monster<LuckybooBangboo>();
        //    ModelDb.Monster<EousBangboo>();
        //}


        public async static Task<Creature> SummonBangboo<T>(PlayerChoiceContext choiceContext,Player player, decimal maxHp, MinionPosition position = MinionPosition.Front, CardModel card = null, decimal PrimaryAmount = 0, decimal SecondAmout = 0m) where T : MinionModel
        {
            return await MinionCmd.AddMinion<T>(choiceContext,player, new MinionSummonOptions(
                    MaxHp: maxHp,                              // 血量
                    PrimaryStatAmount: PrimaryAmount,                  // 主要参数（具体内容在随从的 OnSummon 里定义），还有次要参数等可以按需传入
                    Source: card,                           // 召唤来源（通常是这张牌）
                    Position: position));
        }

    }
}
