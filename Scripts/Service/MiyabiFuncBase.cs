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
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Commands;
using MinionLib.Minion;
using Miyabists2.Scripts.Bangboo.BangbooRelic;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Drawing;
using System.Linq;
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









        // 随从位置偏移，参考 MinionCmd
        private static readonly Vector2 MinionOffset = new Vector2(250f, 25f);

        private static readonly Vector2[] SlotOffsets = new Vector2[4]
        {
            new Vector2(500f, -150f),
            new Vector2(-150f, -150f),
            new Vector2(-200f, 50f),
            new Vector2(450f, 50f)
        };

        /// <summary>
        /// 将怪物作为随从召唤到玩家侧，参考 MinionCmd.AddMinion 实现。
        /// 由 TonghuaJishibenRelic 等遗物调用。
        /// </summary>
        /// <param name="context">PlayerChoiceContext</param>
        /// <param name="recordedMonsterType">已记录怪物的 Type.FullName</param>
        /// <param name="player">召唤者</param>
        /// <param name="maxHp">最大生命值，传 1 则使用怪物的默认 MinInitialHp</param>
        /// <param name="currentHp">当前生命值，传 1 则使用怪物的默认 MinInitialHp</param>
        public static async Task<Creature?> AddMonsterAsPet(
            PlayerChoiceContext context,
            string recordedMonsterType,
            Player player,
            int maxHp = 1,
            int currentHp = 1
        )
        {
            ArgumentNullException.ThrowIfNull(recordedMonsterType);
            ArgumentNullException.ThrowIfNull(player);

            // --- 通过 Type.FullName 反射还原 MonsterModel ---
            Type? monsterType = Type.GetType(recordedMonsterType);
            if (monsterType == null)
            {
                // Type.GetType 对非 mscorlib 类型需要 assembly-qualified name，
                // 兜底搜索所有已加载程序集
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    monsterType = asm.GetType(recordedMonsterType);
                    if (monsterType != null) break;
                }
            }
            if (monsterType == null)
                return null;

            // ModelDb.Monster<T>() 泛型调用
            MethodInfo monsterMethod = typeof(ModelDb).GetMethods()
                .FirstOrDefault(m => m.Name == "Monster"
                    && m.IsGenericMethod
                    && m.GetParameters().Length == 0);
            if (monsterMethod == null)
                return null;

            MethodInfo genericMethod = monsterMethod.MakeGenericMethod(monsterType);
            MonsterModel recordedMonster = (MonsterModel)genericMethod.Invoke(null, null);
            // --- 还原结束 ---

            Creature? pet = player.Creature.CombatState?.CreateCreature(
                recordedMonster.CanonicalInstance.ToMutable(),
                CombatSide.Player,
                null
            );

            if (pet == null)
            {
                return null;
            }


            await CreatureCmd.Add(pet);

            player.PlayerCombatState?.AddPetInternal(pet);

            // 设置随从位置（玩家位置 + 偏移），并翻转精灵朝向
            NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (NCombatRoom.Instance != null && nCreature != null)
            {
                ((Control)nCreature).Position =
                    ((Control)NCombatRoom.Instance.GetCreatureNode(player.Creature)).Position + MinionOffset;
                await FlipScale(NCombatRoom.Instance?.GetCreatureNode(pet)?.Body);
            }

            await PowerCmd.Apply<SummonMonsterPower>(context, pet, 1m, (Creature)null, (CardModel)null, false);
            await pet.Monster.AfterAddedToRoom();

            // 设置血量：默认使用怪物自身的初始血量
            int finalMaxHp = maxHp == 1 ? pet.Monster.MinInitialHp : maxHp;
            int finalCurrentHp = currentHp == 1 ? pet.Monster.MinInitialHp : currentHp;
            await CreatureCmd.SetMaxHp(pet, finalMaxHp);
            await CreatureCmd.SetCurrentHp(pet, finalCurrentHp);

            return pet;
        }

        /// <summary>
        /// 水平翻转精灵（使怪物朝向玩家侧），参考 MinionCmd.FlipScale
        /// </summary>
        public static Task FlipScale(Node2D? body)
        {
            if (body == null)
            {
                return Task.CompletedTask;
            }
            body.Scale *= new Vector2(-1f, 1f);
            return Task.CompletedTask;
        }

        public static async Task PerformPetMonsterMove(Creature pet)
        {
            MonsterModel monster = pet.Monster;

            if (monster.CombatState == null)
            {
                return;
            }

            ICombatState combatState = monster.CombatState;

            await Cmd.CustomScaledWait(0.1f, 0.2f);

            bool IsPerformingMove = true;

            try
            {
                MoveState move = monster.NextMove;

                IReadOnlyList<Creature> targets = combatState.HittableEnemies;

                GD.Print(
                    $"Pet monster {monster.Id.Entry} performing move {move.Id}"
                );

                await move.PerformMove(targets);

                monster.MoveStateMachine?.OnMovePerformed(move);

                CombatManager.Instance.History.MonsterPerformedMove(
                    combatState,
                    monster,
                    move,
                    targets
                );

                if (pet.IsDead &&
                    Hook.ShouldCreatureBeRemovedFromCombatAfterDeath(combatState, pet))
                {
                    combatState.RemoveCreature(pet);
                }
            }
            finally
            {
                IsPerformingMove = false;
            }

            await Cmd.CustomScaledWait(0.1f, 0.4f);
        }

        //public static async Task<Creature> AddMonsterPet<T>(Player player) where T : MonsterModel
        //{
        //    ArgumentNullException.ThrowIfNull(player);

        //    Creature pet = await PlayerCmd.AddPet<T>(player);

        //    PetOrderSnapshotManager.TakeSnapshot(player);
        //    await MinionAnimCmd.Rearrange();

        //    return pet;
        //}

        //private class SummonMinion : MinionModel
        //{
        //    int _minInitialHp = 1;
        //    int _maxInitialHp = 1;
        //    public override int MinInitialHp => _minInitialHp;
        //    public override int MaxInitialHp => _maxInitialHp;

        //    private readonly MonsterModel _innerMonster;

        //    public SummonMinion(MonsterModel monster)
        //    {
        //        _innerMonster = monster;
        //        _minInitialHp = monster.MinInitialHp;
        //        _maxInitialHp = monster.MaxInitialHp;
        //    }

        //    public override string DeathSfx => _innerMonster.DeathSfx;

        //    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        //    {
        //        var method = typeof(MonsterModel).GetMethod("GenerateMoveStateMachine", BindingFlags.NonPublic | BindingFlags.Instance);
        //        return (MonsterMoveStateMachine)method.Invoke(_innerMonster, null);
        //    }
        //}

        //public static async Task<MinionModel> AddMonsterAsMinion(PlayerChoiceContext choiceContext, MonsterModel monster) 
        //{ 
        //    if (monster == null) return null;
        //    SummonMinion minion = new SummonMinion(monster);
        //    return minion;
        //}

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
