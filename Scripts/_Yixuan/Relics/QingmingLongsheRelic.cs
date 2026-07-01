using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Yixuan.Relics
{
    /// <summary>
    /// 青溟笼舍（Rare遗物）：
    /// 每场战斗开始时，随机获取 1 个上一场战斗结束时的正面能力及其层数（不包括闪能）。
    /// 每累计受到100%的生命伤害，额外获取1个。
    /// </summary>
    [RegisterRelic(typeof(YixuanRelicPool))]
    internal class QingmingLongsheRelic : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Rare;

        // TODO: 替换为Yixuan专属遗物图标
        public override string PackedIconPath => "res://images/_YiXuan/relics/qingminLongshe.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        //protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        //[
        //    HoverTipFactory.FromPower<ShannengPower>(),
        //];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Get",1),
            new DynamicVar("Record",0),
        ];

        // ========== Saved State ==========

        /// <summary>
        /// 上一场战斗结束时保存的正面能力数据。
        /// 格式：typeFullName:amount;typeFullName:amount;...
        /// 用分号分隔不同能力，冒号分隔类型名和层数
        /// </summary>
        private string _savedPowersData = "";

        [SavedProperty]
        public string SavedPowersData
        {
            get => _savedPowersData;
            private set
            {
                AssertMutable();
                _savedPowersData = value ?? "";
            }
        }

        /// <summary>本局战斗累计受到的未格挡伤害（用于计算额外抽取次数）</summary>
        private int _accumulatedDamage = 0;

        [SavedProperty]
        public int AccumulatedDamage
        {
            get => _accumulatedDamage;
            private set
            {
                AssertMutable();
                _accumulatedDamage = value;
                DynamicVars["Record"].BaseValue = AccumulatedDamage;
            }
        }

        private int _getCount = 1; // 基础1次抽取 + 累计伤害额外抽取
        [SavedProperty]
        public int GetCount
        {
            get => _getCount;
            private set
            {
                AssertMutable();
                _getCount = value;
                DynamicVars["Get"].BaseValue = GetCount;
            }
        }

        // ========== Hooks ==========

        /// <summary>战斗胜利后：保存当前所有正面能力（不含闪能）及其层数</summary>
        //public override async Task AfterCombatVictoryEarly(CombatRoom room)
        //{
        //    GD.Print($"[QingmingLongsheRelic] AfterCombatVictory called. room={room.RoomType}");
        //    SaveCurrentPositivePowers();
        //    GD.Print($"[QingmingLongsheRelic] AfterCombatVictory done. SavedPowersData='{SavedPowersData}'");
        //}

        public override bool ShouldStopCombatFromEnding()
        {
            SaveCurrentPositivePowers();
            GD.Print($"[QingmingLongsheRelic] AfterCombatVictory done. SavedPowersData='{SavedPowersData}'");
            return false;
        }

        /// <summary>
        /// 每场战斗第一回合开始时：
        /// 基础1次抽取 + 每累计100%生命伤害额外1次，
        /// 从上一场战斗保存的能力池中随机抽取并附加给玩家。
        /// 先不重复抽取所有不重复能力，若抽取次数多于能力数则允许重复。
        /// </summary>
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (Owner.Creature.CombatState.RoundNumber != 1) return;

            GetCount = 1 + CalculateBonusDraws();
            DynamicVars["Get"].BaseValue = GetCount;
            DynamicVars["Record"].BaseValue = AccumulatedDamage;

            GD.Print($"[QingmingLongsheRelic] AfterPlayerTurnStart: GetCount={GetCount}, AccumulatedDamage={AccumulatedDamage}");
            GD.Print(SavedPowersData);

            // 重置累积伤害（新战斗开始重新计算）
            //AccumulatedDamage = 0;

            if (string.IsNullOrEmpty(SavedPowersData)) return;
            if (GetCount <= 0) return;

            Flash();
            await DrawAndApplyPowers(choiceContext, GetCount);
        }

        /// <summary>受到伤害后：累计未格挡伤害用于计算额外抽取次数</summary>
        public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner?.Creature) return;
            //if (!CombatManager.Instance.IsInProgress) return;
            //if (result.UnblockedDamage <= 0) return;

            AccumulatedDamage += 100 * result.UnblockedDamage / Owner.Creature.MaxHp;
        }

        // ========== Private Methods ==========

        /// <summary>读取玩家身上所有非闪能的Buff型能力及其层数，保存到字符串中</summary>
        private void SaveCurrentPositivePowers()
        {
            var creature = Owner?.Creature;
            if (creature == null)
            {
                GD.Print("[QingmingLongsheRelic] SaveCurrentPositivePowers: creature is null!");
                return;
            }

            var powers = creature.Powers.ToList();
            GD.Print($"[QingmingLongsheRelic] SaveCurrentPositivePowers: creature has {powers.Count} powers total");

            var entries = new List<string>();

            foreach (var power in powers)
            {
                GD.Print($"[QingmingLongsheRelic]   power: {power.GetType().Name}, Type={power.Type}, Amount={power.Amount}");
                // 排除闪能和支援点数
                if (power is ShannengPower) { GD.Print($"    -> skipped (ShannengPower)"); continue; }
                if (power is SupportPointPower) { GD.Print($"    -> skipped (SupportPointPower)"); continue; }
                // 只保留正面Buff型能力
                if (power.Type != PowerType.Buff) { GD.Print($"    -> skipped (not Buff, Type={power.Type})"); continue; }

                entries.Add($"{power.GetType().FullName}:{power.Amount}");
                GD.Print($"    -> SAVED: {power.GetType().FullName}:{power.Amount}");
            }

            SavedPowersData = string.Join(";", entries);
            GD.Print($"[QingmingLongsheRelic] SaveCurrentPositivePowers: final SavedPowersData='{SavedPowersData}'");
        }

        /// <summary>根据累计伤害计算额外抽取次数：每100%最大生命值伤害 = +1次</summary>
        private int CalculateBonusDraws()
        {
            if (Owner?.Creature == null) return 0;
            var maxHp = Owner.Creature.MaxHp;
            if (maxHp <= 0) return 0;

            return (int)(AccumulatedDamage / 100);
        }

        /// <summary>
        /// 从保存的能力池中随机抽取 totalDraws 次并应用。
        /// 先不重复地抽取，若池中能力不足则重置池子允许重复抽取。
        /// </summary>
        private async Task DrawAndApplyPowers(PlayerChoiceContext choiceContext, int totalDraws)
        {
            var creature = Owner.Creature;
            var rng = Owner.RunState.Rng.Shuffle;

            // 解析保存的能力数据
            var powerEntries = ParseSavedPowers();

            // 可用索引池：从中随机选取（不放回）
            var availableIndices = Enumerable.Range(0, powerEntries.Count).ToList();
            var drawnCount = 0;

            while (drawnCount < totalDraws)
            {
                if (availableIndices.Count == 0)
                {
                    // 所有能力都已抽过一轮，重置池子（之后允许重复）
                    availableIndices = Enumerable.Range(0, powerEntries.Count).ToList();
                    if (availableIndices.Count == 0) break; // 池子为空，安全退出
                }

                // 随机选取一个索引
                int pick = rng.NextInt(0, availableIndices.Count);
                int index = availableIndices[pick];
                availableIndices.RemoveAt(pick);

                var entry = powerEntries[index];
                // 应用该能力
                await ApplyPowerByTypeName(
                    choiceContext, creature,
                    entry.TypeName, entry.Amount);

                drawnCount++;
            }
        }

        /// <summary>解析 SavedPowersData 字符串为 (TypeName, Amount) 列表</summary>
        private List<(string TypeName, int Amount)> ParseSavedPowers()
        {
            var result = new List<(string TypeName, int Amount)>();
            if (string.IsNullOrEmpty(SavedPowersData)) return result;

            var entries = SavedPowersData.Split(';');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var amount))
                {
                    result.Add((parts[0], amount));
                }
            }
            return result;
        }

        /// <summary>通过反射动态调用 PowerCmd.Apply&lt;T&gt; 来附加指定类型的能力</summary>
        private static async Task ApplyPowerByTypeName(PlayerChoiceContext choiceContext, Creature creature, string typeName, int amount)
        {
            try
            {
                GD.Print($"[QingmingLongsheRelic] ApplyPowerByTypeName: trying '{typeName}' x{amount}");

                var powerType = ResolvePowerType(typeName);
                if (powerType == null)
                {
                    GD.PrintErr($"[QingmingLongsheRelic] ApplyPowerByTypeName: FAILED to resolve type '{typeName}'");
                    return;
                }
                GD.Print($"[QingmingLongsheRelic] ApplyPowerByTypeName: resolved to {powerType.FullName}");

                // 反射调用 PowerCmd.Apply<T>(choiceContext, creature, amount, null, null, false)
                // 需要精确匹配参数类型，因为有多个 Apply<T> 重载（如接受 IEnumerable<Creature> 的版本）
                var applyMethod = typeof(PowerCmd)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "Apply"
                        && m.IsGenericMethod
                        && m.GetParameters().Length == 6
                        && m.GetParameters()[1].ParameterType == typeof(Creature));

                if (applyMethod == null)
                {
                    GD.PrintErr("[QingmingLongsheRelic] ApplyPowerByTypeName: FAILED to find PowerCmd.Apply method!");
                    return;
                }
                GD.Print($"[QingmingLongsheRelic] ApplyPowerByTypeName: found Apply method");

                var genericMethod = applyMethod.MakeGenericMethod(powerType);
                var task = (Task)genericMethod.Invoke(null, new object?[] { choiceContext, creature, (decimal)amount, null, null, false });
                await task;
                GD.Print($"[QingmingLongsheRelic] ApplyPowerByTypeName: SUCCESS '{typeName}' x{amount}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[QingmingLongsheRelic] Failed to apply power '{typeName}': {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    GD.PrintErr($"[QingmingLongsheRelic]   Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
        }

        /// <summary>根据类型全名解析 Power 类型</summary>
        private static Type? ResolvePowerType(string typeName)
        {
            // 先在已加载的程序集中查找
            var powerType = Type.GetType(typeName);
            if (powerType != null) return powerType;

            // 回退：遍历所有程序集查找
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    powerType = asm.GetType(typeName);
                    if (powerType != null) return powerType;
                }
                catch
                {
                    // 某些程序集可能无法访问，忽略
                }
            }

            return null;
        }
    }
}
