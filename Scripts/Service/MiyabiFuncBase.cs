using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Drawing;
using System.Reflection;
using static Godot.XmlParser;

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

        public static int RandomInt(int Min , int exMax, Player player)
        {
            int result = player.RunState.Rng.Shuffle.NextInt(Min, exMax);
            return result;
        }

        public static bool GetIsTrue100(int trueRate, Player player)
        {
            int randomValue = RandomInt(1, 101, player);
            if (randomValue <= trueRate)
                return true;
            else
                return false;
        }

        public static RelicModel GetRelic<T>(Player player) where T : RelicModel
        {
            return player.Relics.OfType<T>().FirstOrDefault();
        }

        public static async Task<IEnumerable<CardPileAddResult>> AddCardToDesk<T>(Player player, float showTime = 2f) where T : CardModel
        {
            GD.Print($"[AddCardToDesk] 开始 — T={typeof(T).Name}, player={player.Character?.GetType().Name}");

            List<CardPileAddResult> results = new List<CardPileAddResult>();
            CardModel card = player.RunState.CreateCard(ModelDb.Card<PriceOfPower>(), player);
            GD.Print($"[AddCardToDesk] CreateCard 完成 — cardType={card.GetType().Name}, is PriceOfPower={card is PriceOfPower}, Owner={card.Owner != null}");

            results.Add(await CardPileCmd.Add(card, PileType.Deck));
            GD.Print($"[AddCardToDesk] CardPileCmd.Add 完成 — results.Count={results.Count}");

            CardCmd.PreviewCardPileAdd(results, showTime);
            GD.Print($"[AddCardToDesk] 结束");

            return results;
        }


        public static bool IsMiyabiModChar(Player player) 
        {
            if(player == null) return false;

            return player.Character is Miyabi || player.Character is Yixuan;
        }

        public static LocString GetForMonsterString(string entryName)
        {
            return new LocString("monsters", entryName+".banter");
        }

        public static bool isModEffectApply(Player player)
        {
            return IsMiyabiModChar(player) || MiyabiModConfig.ChangeToAllPlayers;
        }

        //public static bool isEnemyAppear(ActModel act)
        //{
        //    return IsMiyabiModChar(player) || MiyabiModConfig.MiyabiEnemiesAppearWhenPlayingOtherChar;
        //}


        //public static void AddRelicToChar<T>(RelicModel r) where T : RelicPoolModel
        //{
        //    // 1. 先拿到官方 AddModelToPool 的方法信息
        //    MethodInfo method = typeof(ModHelper).GetMethod(
        //        nameof(ModHelper.AddModelToPool),
        //        BindingFlags.Public | BindingFlags.Static
        //    );

        //    if (method == null) return;

        //        // 2. 🎯 核心魔法：动态把编译期的 T 和你循环里的 types 融合成双泛型
        //        // 相当于在运行时强行把 types 塞进了尖括号
        //    MethodInfo genericMethod = method.MakeGenericMethod(typeof(T), r.GetType());

        //        // 3. 执行这个动态生成的方法
        //    genericMethod.Invoke(null, null);
        //}

        //通用PlayerChoiceContext
        //public static PlayerChoiceContext choiceContext = new HookPlayerChoiceContext(Owner, Owner.NetId, MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Any);
    }

    //public static class ActModelExtensions
    //{
    //    public static int ActNumber(this ActModel actModel)
    //    {
    //        if (!(actModel is Overgrowth) && !(actModel is Underdocks))
    //        {
    //            if (!(actModel is Hive))
    //            {
    //                if (!(actModel is Glory))
    //                {
    //                    GD.Print("[MiyabiMod] ActNumber Unknown act type,setting to -1");
    //                    return -1;
    //                }

    //                return 3;
    //            }

    //            return 2;
    //        }

    //        return 1;
    //    }
    //}
}
