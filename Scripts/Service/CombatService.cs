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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System.Drawing;
using System.Reflection;

namespace Miyabists2.Scripts.Service
{
    public class MiyabiCombatService
    {
        //伙伴卡牌的特殊处理
        //public static bool ThisTurnUsedPartnerCard { set; get; } = false;
        //public static bool GetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard;
        //public static void ResetThisTurnUsedPartnerCard() => ThisTurnUsedPartnerCard = false;
        //public static void UsedPartnerCard() => ThisTurnUsedPartnerCard = true;
        public static void SetSupportCostToZero(PlayerChoiceContext choiceContext, CardModel card, Player owner)
        {
            if(owner.Character is Miyabi)
            {
                
            }

            if (owner.Character is Yixuan)
            {

            }
        }

        //bangboo相关
        public static bool IsBangbooOnField(Player player)
        {
            if (player == null) return false;

            bool b = player.Creature.Pets.Any(
                pet => pet.IsMonster &&
                pet.Monster is MiyabiBangbooBase);

            // 假设你的邦布随从类叫 BangbooMinion，或者可以通过 ID/Tag 判断
            // 这里根据你 Mod 的具体实现来改
            GD.Print($"[MiyabiBangboo] : {player.Character.Title.ToString()} has any Bangboo? : {b.ToString()}");
            return b;
        }

        /// <summary>
        /// 邦布随机召唤
        /// </summary>
        public static async Task SummonBangbooRandom(PlayerChoiceContext choiceContext, Player Owner)
        {
            int result = MiyabiFuncBase.RandomInt(0, 12, Owner);

            if (result == 0)
            {
                await MiyabiBangbooService.SummonBangboo<EousBangboo>(choiceContext, Owner, 6m, MinionPosition.Back, null, 1m);
            }
            else if (result == 1)
            {
                await MiyabiBangbooService.SummonBangboo<LuckybooBangboo>(choiceContext, Owner, 4m, MinionPosition.FrontUpper, null, 1m);
            }
            else if (result == 2)
            {
                await MiyabiBangbooService.SummonBangboo<ExplorebooBangboo>(choiceContext, Owner, 4m, MinionPosition.Back);
            }
            else if (result == 3)
            {
                await MiyabiBangbooService.SummonBangboo<SumobooBangboo>(choiceContext, Owner, 4m, MinionPosition.Front, null, 1m);
            }
            else if (result == 4)
            {
                await MiyabiBangbooService.SummonBangboo<PaperbooBangboo>(choiceContext, Owner, 15m, MinionPosition.Front, null, 1m);
            }
            else if (result == 5)
            {
                await MiyabiBangbooService.SummonBangboo<OvertimebooBangboo>(choiceContext, Owner, 3m, MinionPosition.Back, null, 1m);
            }
            else if (result == 6)
            {
                await MiyabiBangbooService.SummonBangboo<SharkbooBangboo>(choiceContext, Owner, 8m, MinionPosition.FrontUpper, null, 1m);
            }
            else if (result == 7)
            {
                await MiyabiBangbooService.SummonBangboo<ExcalibooBangboo>(choiceContext, Owner, 8m, MinionPosition.BackUpper);
            }
            else if (result == 8)
            {
                await MiyabiBangbooService.SummonBangboo<AgentBangboo>(choiceContext, Owner, 10m, MinionPosition.FrontUpper);
            }
            else if (result == 9)
            {
                await MiyabiBangbooService.SummonBangboo<MagnetibooBangboo>(choiceContext, Owner, 8m, MinionPosition.FrontUpper, null, 1m);
            }
            else if (result == 10)
            {
                await MiyabiBangbooService.SummonBangboo<OneDennybooBangboo>(choiceContext, Owner, 4m, MinionPosition.FrontUpper, null, 1m);
            }
            else if (result == 11)
            {
                await MiyabiBangbooService.SummonBangboo<XixifuBangboo>(choiceContext, Owner, 8m, MinionPosition.Back, null, 1m);
            }
        }


        public static bool AnoNeedCheck { get; set; } = true;
        public static bool DazeNeedCheck { get; set; } = true;
        public static bool FrostNeedCheck { get; set; } = true;

        public static void ResetCheck()
        {
            AnoNeedCheck = true;
            DazeNeedCheck = true;
            FrostNeedCheck = true;
        }

        //属性积蓄判断
        private static int _anoTrigger = 5;
        public static int AnoTrigger { get; set; } = _anoTrigger;
        public static void ChangeAnoT(int amount) => AnoTrigger += amount;
        public static void ResetAnoT() => AnoTrigger = _anoTrigger;
        public static int GetAnoTrigger() => AnoTrigger;

        public static void SetAnoTriggerMultiply(Creature c)
        {

            int mul = c.CombatState.PlayerCreatures.Count;

            if (AnoNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeAnoT((mul - 1) * 2);
                    AnoNeedCheck = false;
                }
                else
                {
                    ResetAnoT();
                    AnoNeedCheck = false;
                }
            }
            
        }

        //失衡值判断
        private static int _dazeTrigger = 100;
        public static int DazeTrigger { get; set; } = _dazeTrigger;
        public static void ChangeDazeT(int amount) => DazeTrigger += amount;
        public static void ResetDazeT() => DazeTrigger = _dazeTrigger;
        public static int GetDazeTrigger() => DazeTrigger;

        public static void SetDazeTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;

            if (DazeNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeDazeT((mul - 1) * 20);
                    DazeNeedCheck = false;
                }
                else
                {
                    ResetDazeT();
                    DazeNeedCheck = false;
                }
            }
            
        }

        //烈霜值判断
        private static int _frostTrigger = 50;
        public static int FrostTrigger { get; set; } = _frostTrigger;
        public static void ChangeFrostT(int amount) => FrostTrigger += amount;
        public static void ResetFrostT() => FrostTrigger = _frostTrigger;
        public static int GetFrostTrigger() => FrostTrigger;

        public static bool CanAddFrostWhenFire { get; set; } = false;
        public static bool GetCanAddWhenFire() => CanAddFrostWhenFire;
        public static bool SetCanAddWhenFire(bool value) => CanAddFrostWhenFire = value;
        public static void ResetCanAddWhenFire() => CanAddFrostWhenFire = false;


        public static void SetFrostTriggerMultiply(Creature c)
        {
            int mul = c.CombatState.PlayerCreatures.Count;
            if (FrostNeedCheck)
            {
                if (mul > 1)
                {
                    ChangeFrostT((mul - 1) * 15);
                    FrostNeedCheck = false;
                }
                else
                {
                    ResetFrostT();
                    FrostNeedCheck = false;
                }
            }
        }

        //新月祝福
        public static bool IsAnyHasMoonBlessing(Creature c)
        {
            foreach (Creature Player in c.CombatState.PlayerCreatures)
            {
                if (Player != null && Player.IsAlive && Player.HasPower<BlessingMoonPower>())
                {
                    return true;
                }
                //NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
            return false;
        }

        //烈霜
        public static decimal FrostFireLimit { get; set; } = 0.5m;
        public static decimal GetFrostFireLimit() => FrostFireLimit;
        public static decimal SetFrostFireLimit(decimal value) => FrostFireLimit = value;
        public static void ResetFrostFireLimit() => FrostFireLimit = 0.5m;

        public static async Task AddAnoBuildup(Creature target, int anoVar, Creature dealer, CardModel card, PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            SetAnoTriggerMultiply(target);

            //处理Amount而不是DisplayAmount
            int trigger = GetAnoTrigger() + 1;
            bool hasAnomaly = target.HasPower<AttributeAnomalyPower>();
            int chkAno = anoVar;
            if (target.HasPower<AnomalyBuildupPower>())
                chkAno += target.GetPowerAmount<AnomalyBuildupPower>();


            // 统一用循环处理，支持任意大小的积蓄值一次性正确结算
            // 奇数轮施加异常，偶数轮触发紊乱（交替进行）
            if (chkAno >= trigger)
            {
                while (chkAno >= trigger)
                {
                    if (hasAnomaly)
                    {
                        await DisorderApply(target, dealer, choiceContext);
                        hasAnomaly = false;
                    }
                    else
                    {
                        await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, dealer, card);
                        hasAnomaly = true;
                    }
                    chkAno -= trigger;
                }

                // 设置剩余积蓄
                var buildupPower = target.GetPower<AnomalyBuildupPower>();
                int remainder = chkAno + 1;
                if (buildupPower != null)
                {
                    await MiyabiFuncBase.SetPowerAmount(choiceContext, buildupPower, remainder, dealer, null);
                }
                else if (remainder > 0)
                {
                    await PowerCmd.Apply<AnomalyBuildupPower>(choiceContext, target, remainder, dealer, card);
                }
            }
            else // 未满则继续堆积蓄
            {
                await PowerCmd.Apply<AnomalyBuildupPower>(choiceContext, target, anoVar, dealer, card);
            }
        }

        public static decimal DisorderDamageRate { get; set; } = 0.1m;
        public static void ResetDisorderDamageRate() { DisorderDamageRate = 0.1m; }

        //用于调用各种触发
        //紊乱触发
        public static async Task DisorderApply(Creature target, Creature dealer, PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            await PowerCmd.Remove<AttributeAnomalyPower>(target);

            await DealAnoDamage(choiceContext, dealer, target, 10);

            await PowerCmd.Apply<DisorderPower>(choiceContext, target, 1, dealer, null);
        }

        //霜灼增加
        public static async Task FrostApply(Creature target, Creature dealer , PlayerChoiceContext choiceContext)
        {
            if (target == null || target.IsDead) return;

            bool hasHanyan = false;
            if (dealer.IsPlayer)
                hasHanyan = dealer.Player.PlayerCombatState.AllCards.Any(c => c is ShenxueHanyan);

            SetFrostTriggerMultiply(target);

            await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<FrostBuildPower>(), 1, dealer, null);
            
            if(hasHanyan)
            {
                await PowerCmd.Apply<FrostpoPower>(choiceContext, target, 1, dealer, null);
                int fireAmount = target.GetPowerAmount<FrostFirePower>();

                await CreatureCmd.Damage(choiceContext, target, fireAmount * 5m, ValueProp.Unpowered, dealer);
                if(fireAmount / 5m >= 1)
                    await PowerCmd.Apply<StrengthPower>(choiceContext, dealer, fireAmount / 5m, dealer, null);

                await PowerCmd.Remove(target.GetPower<FrostFirePower>());
            }
            else
            {
                await PowerCmd.Apply<FrostPower>(choiceContext, target, 1, dealer, null);
            }



            if (target.HasPower<AttributeAnomalyPower>())
            {
                await DisorderApply(target, dealer, choiceContext);
            }
            else
            {
                await PowerCmd.Apply<AttributeAnomalyPower>(choiceContext, target, 1, dealer, null);
            }
        }

        public static async Task DealAnoDamage(PlayerChoiceContext choiceContext, Creature dealer, Creature target, int perD, decimal f = 100)
        {
            bool moonBless = false;
            if (dealer != null && dealer.IsPlayer)
            {
                moonBless = IsAnyHasMoonBlessing(dealer);
            }
            bool hasZmyc = target.HasPower<ZhongmuycPower>();

            decimal percent = ((decimal)perD) / 100m;
            if (moonBless)
                percent += 0.05m;
            if (target.HasPower<YaobianPower>())
            {
                percent += ((decimal)target.GetPowerAmount<YaobianPower>()) / 100m;
            }

            //造成属性异常伤害
            decimal damage = target.MaxHp * percent;

            if (hasZmyc) damage *= 1.5m;

            damage *= f / 100;

            await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Unpowered | ValueProp.Unblockable, dealer);
        }


        //失衡值叠加
        public static async Task AddDaze(PlayerChoiceContext choiceContext, Creature target,DynamicVar dazeVar,Creature dealer)
        {
            if (target == null || target.IsDead) return;

            SetDazeTriggerMultiply(target);

            int chkDaze = target.GetPowerAmount<DazePower>() + dazeVar.IntValue;

            if (!target.HasPower<BreakPower>() && chkDaze <= DazeTrigger)
                await PowerCmd.Apply<DazePower>(choiceContext, target, dazeVar.BaseValue, dealer, null);
            else if (chkDaze >= DazeTrigger + 1)
            {
                //await PowerCmd.SetAmount<DazePower>(target, 1, dealer, null);
                await MiyabiFuncBase.SetPowerAmount(choiceContext, target.GetPower<DazePower>(), 1, dealer, null);
                await PowerCmd.Apply<BreakPower>(choiceContext, target, 1, dealer, null);
            }
        }

        public static async Task DazeAddtoPlayer(PlayerChoiceContext choiceContext, Creature player, int count)
        {
            if (player == null || player.IsDead) return;

            int chkDaze = count;
            if (player.HasPower<DazePower>())
            {
                chkDaze += player.GetPowerAmount<DazePower>();
            }
            
            if(chkDaze >= 100 && !player.HasPower<BreakPlayerPower>())
            {
                await PowerCmd.Apply<BreakPlayerPower>(choiceContext, player, 1, null, null);
                await PowerCmd.Remove<DazePower>(player);
            }
            else if(!player.HasPower<BreakPlayerPower>())
            {
                await PowerCmd.Apply<DazePower>(choiceContext, player, count, null, null);
            }

        }

        //加喧嚣值
        public static async Task AddDecible(Player player, int amount)
        {
            // 查找所有实现IDecibleCounter接口的遗物（包括Miyabi/Yixuan/Guang的喧嚣值遗物）
            var decibleRelics = player.Relics.OfType<IDecibleCounter>();

            foreach (var relic in decibleRelics)
            {
                relic.AddCounter(amount);
            }
        }

        //花辞
        public static async Task AddHuaCiReward(Creature owner, Creature target, PlayerChoiceContext choiceContext, int Count)
        {
            int handSize = owner.Player.PlayerCombatState.Hand.Cards.Count;
            if (target == null)
                target = owner.CombatState.HittableEnemies.TakeRandom(1, owner.Player.RunState.Rng.CombatCardSelection).FirstOrDefault();

            for (int i = 0; i < Count; i++)
            {
                CardModel reward1 = owner.CombatState.CreateCard<HuaCi>(owner.Player);

                if (handSize + i <= RitsuLibFramework.GetMaxHandSize(owner.Player))
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, owner.Player);
                else
                    await CardCmd.AutoPlay(choiceContext, reward1, target);
            }
        }

        /// <summary>
        /// 通用资源选择机制：根据传入的卡片Type和对应的执行方法映射，
        /// 为每个Type生成选择卡片并展示选择界面，玩家选择后执行对应的回调。
        /// 参考 RelicClick.OnPlay 的选择模式。
        /// </summary>
        /// <param name="choiceContext">选择上下文</param>
        /// <param name="owner">执行选择的玩家</param>
        /// <param name="typeActions">卡片Type → 执行方法的映射；Key为选择卡片类型，Value为当选中该类型时执行的异步回调</param>
        /// <param name="onCardCreated">可选回调，每张卡片创建后调用，用于在展示前对卡片进行额外配置（如SetAmount等）</param>
        public static async Task ChooseRes(PlayerChoiceContext choiceContext, Player owner, Dictionary<Type, Func<PlayerChoiceContext, Task>> typeActions, Action<CardModel> onCardCreated = null)
        {
            if (typeActions == null || typeActions.Count == 0) return;

            var combatState = owner.Creature?.CombatState;
            if (combatState == null) return;

            // 通过反射获取泛型 CreateCard<T>(Player) 方法
            var createCardMethod = typeof(CombatState)
                .GetMethods()
                .FirstOrDefault(m => m.Name == "CreateCard"
                    && m.IsGenericMethod
                    && m.GetParameters().Length == 1);

            if (createCardMethod == null)
            {
                GD.PrintErr("[MiyabiCombatService.ChooseRes] CreateCard method not found on CombatState");
                return;
            }

            // 为每个Type创建对应的选择卡片实例
            List<CardModel> options = new List<CardModel>();
            foreach (var type in typeActions.Keys)
            {
                try
                {
                    var genericMethod = createCardMethod.MakeGenericMethod(type);
                    var card = genericMethod.Invoke(combatState, new object[] { owner }) as CardModel;
                    if (card != null)
                    {
                        onCardCreated?.Invoke(card);
                        options.Add(card);
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[MiyabiCombatService.ChooseRes] Failed to create card of type {type.Name}: {ex.Message}");
                }
            }

            if (options.Count == 0) return;

            // 展示选择界面
            CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, owner);

            // 根据玩家选择的卡片类型，执行对应的回调方法
            if (chosen != null && typeActions.TryGetValue(chosen.GetType(), out var action))
            {
                await action(choiceContext);
            }
        }

        /// <summary>
        /// Yixuan专用资源选择（带数值）：在ChooseRes基础上，
        /// 为每个选择卡片调用SetAmount(int)设置显示数值。
        /// 参考 VigorChoice / ThornsChoice 的 SetAmount 用法。
        /// </summary>
        /// <param name="choiceContext">选择上下文</param>
        /// <param name="owner">执行选择的玩家</param>
        /// <param name="typeActions">卡片Type → (数值Amount, 选中后执行的异步回调) 的映射</param>
        public static async Task ChooseResYi(PlayerChoiceContext choiceContext, Player owner, Dictionary<Type, (int Amount, Func<PlayerChoiceContext, Task> Action)> typeActions)
        {
            if (typeActions == null || typeActions.Count == 0) return;

            // 分离出纯 action 字典供 ChooseRes 使用
            var actionMap = typeActions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Action);

            // 通过 onCardCreated 回调，为卡片调用 SetAmount 设置数值
            await ChooseRes(choiceContext, owner, actionMap, card =>
            {
                var cardType = card.GetType();
                if (typeActions.TryGetValue(cardType, out var entry))
                {
                    // 反射调用 SetAmount(int) —— VigorChoice / ThornsChoice 均定义此方法
                    var setAmountMethod = cardType.GetMethod("SetAmount");
                    setAmountMethod?.Invoke(card, new object[] { entry.Amount });
                }
            });
        }
    }
}
