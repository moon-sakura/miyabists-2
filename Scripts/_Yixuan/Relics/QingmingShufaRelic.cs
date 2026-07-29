using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts._Yixuan.Cards.NoneShow;
using Miyabists2.Scripts._Yixuan.Powers;
using Miyabists2.Scripts.Cards;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Ui.Toast;

namespace Miyabists2.Scripts._Yixuan.Relics
{
    [RegisterRelic(typeof(YixuanRelicPool))]
    [RegisterTouchOfOrobasRefinement(typeof(QingmingNiaoRelic))]
    internal class QingmingShufaRelic : ModRelicTemplate, IDecibleCounter
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        // TODO: 替换为Yixuan专属遗物图标
        public override string PackedIconPath => "res://images/_YiXuan/char/common.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        // TODO: 替换为Yixuan专属触发卡和关键字
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<QingmingYunying>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        public int Threshold { get; set; } = 30; // 触发阈值
        public int Max { get; set; } = 30;

        private int _counter;

        // 显示在遗物图标上的数字
        public override bool ShowCounter => true;
        public override int DisplayAmount => Counter;

        [SavedProperty]
        public int Counter
        {
            get => _counter;
            private set
            {
                AssertMutable();
                _counter = value;
                InvokeDisplayAmountChanged();
            }
        }

        public void AddCounter(int amount, bool forceAdd = false)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is QingmingYunying);
            if (hasEnd) counter += Threshold;

            if (counter < Max || forceAdd)
                this.Counter += amount;
        }

        public void SetMax(int amount) => Max = amount;
        public void ResetMax() => Max = 30;

        public int GetCounter() => Counter;

        // 每次打出卡牌后检查
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is QingmingYunying);
            if (hasEnd) counter += Threshold;

            if (cardPlay.Card.Owner == base.Owner)
            {
                if (counter < Max)
                {
                    Counter++;
                }

                // 检查是否达到阈值
                if (Counter >= Threshold && !hasEnd)
                {
                    if (base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is FufaQianchong))
                        return;

                    Counter -= Threshold;
                    if (Counter > Max - Threshold)
                        Counter = Max - Threshold;

                    Flash();

                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<QingmingYunying>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner);
                }

            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                //第一回合添加能力
                (await PowerCmd.Apply<ShannengPower>(choiceContext, base.Owner.Creature, 151, null, null)).SetUsed(ShannengUsed);
            }
        }

        private int _shannengUsed;

        [SavedProperty]
        public int ShannengUsed
        {
            get => _shannengUsed;
            private set
            {
                AssertMutable();
                _shannengUsed = value;
            }
        }

        public void SetUsed(int used) => ShannengUsed = used;

        public void SetThreshold(int threshold) => Threshold = threshold;
        public void ResetThreshold() => Threshold = 30;

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            ResetThreshold();
        }

    }

    /// <summary>
    /// 清明书法遗物的升级版（Orobas精炼后）
    /// TODO: 替换为Yixuan专属效果
    /// </summary>
    [RegisterRelic(typeof(YixuanRelicPool))]
    internal class QingmingNiaoRelic : ModRelicTemplate, IDecibleCounter, IModRightClickableRelic
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => "res://images/_YiXuan/relics/qingmingNiao.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<QingmingYunying>(),
            HoverTipFactory.FromKeyword(MiyabiKeywords.EndSkill),
            HoverTipFactory.FromPower<ShannengPower>(),
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Vigor",2),
            new DynamicVar("Thorns",2),
            new DynamicVar("Shufa",5),
        ];

        public int Threshold { get; set; } = 30;
        public int Max { get; set; } = 30;

        private int _counter;

        public override bool ShowCounter => true;
        public override int DisplayAmount => Counter;

        [SavedProperty]
        public int Counter
        {
            get => _counter;
            private set
            {
                AssertMutable();
                _counter = value;
                InvokeDisplayAmountChanged();
            }
        }

        public void AddCounter(int amount, bool forceAdd = false)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is QingmingYunying);
            if (hasEnd) counter += Threshold;

            if (counter < Max || forceAdd)
                this.Counter += amount;
        }

        public void SetMax(int amount) => Max = amount;
        public void ResetMax() => Max = 30;

        public int GetCounter() => Counter;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            int counter = Counter;
            bool hasEnd = base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is QingmingYunying);
            if (hasEnd) counter += Threshold;

            if (cardPlay.Card.Owner == base.Owner)
            {
                if(counter < Max)
                    Counter++;

                if (Counter >= Threshold && !hasEnd)
                {
                    if (base.Owner.PlayerCombatState.Hand.Cards.Any(c => c is FufaQianchong))
                        return;

                    Counter -= Threshold;
                    if (Counter > Max - Threshold)
                        Counter = Max - Threshold;

                    Flash();

                    CardModel reward1 = base.Owner.Creature.CombatState.CreateCard<QingmingYunying>(base.Owner.Creature.Player);
                    await CardPileCmd.AddGeneratedCardToCombat(reward1, PileType.Hand, Owner);
                }
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner) { return; }
            if (base.Owner.Creature.CombatState.RoundNumber == 1)
            {
                Flash();
                (await PowerCmd.Apply<ShannengPower>(choiceContext, base.Owner.Creature, 151, null, null)).SetUsed(ShannengUsed);
            }
        }

        private int _shannengUsed;

        [SavedProperty]
        public int ShannengUsed
        {
            get => _shannengUsed;
            private set
            {
                AssertMutable();
                _shannengUsed = value;
            }
        }
        public void SetUsed(int used) => ShannengUsed = used;



        // 可选：本地预检，返回 false 则本次右键不会触发
        public bool CanHandleRightClickLocal(ModRightClickContext context)
        {
            return Owner.Creature.CurrentHp > Owner.Creature.MaxHp * 0.05m && CombatManager.Instance.IsInProgress;
        }

        // 右键执行（多人下会在所有客户端同步执行）
        public async Task OnRightClick(ModRightClickExecutionContext context)
        {
            await CreatureCmd.Damage(context.PlayerChoiceContext, Owner.Creature, Owner.Creature.MaxHp * 0.05m, ValueProp.Unpowered | ValueProp.Unblockable, Owner.Creature);
            await GetChoice(context.PlayerChoiceContext);
        }


        public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if(target != Owner.Creature || !CombatManager.Instance.IsInProgress)
            {
                return;
            }

            //GD.Print($"[MiyabiRelic] : Yixuan got hurted form {dealer.LogName}!\\nHook : {choiceContext.GetType().FullName}");

            if (dealer != Owner.Creature && result.UnblockedDamage > 0)
            {
                await ChoosePower(choiceContext, 0, true);
            }


            if (dealer == Owner.Creature && result.UnblockedDamage > 0)
            {
                //await GetChoice(choiceContext);
            }
        }

        public async Task GetChoice(PlayerChoiceContext choiceContext)
        {
            ShufaChoice shufa = base.Owner.Creature.CombatState?.CreateCard<ShufaChoice>(base.Owner);
            VigorChoice vigor = base.Owner.Creature.CombatState?.CreateCard<VigorChoice>(base.Owner);
            ThornsChoice thorns = base.Owner.Creature.CombatState?.CreateCard<ThornsChoice>(base.Owner);


            if (shufa != null && vigor != null && thorns != null)
            {
                List<CardModel> options = new List<CardModel> { shufa, vigor, thorns };
                CardModel chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, base.Owner);
                if (chosen is ShufaChoice)
                {
                    await ChoosePower(choiceContext, 3);
                }
                else if (chosen is VigorChoice)
                {
                    await ChoosePower(choiceContext, 1);
                }
                else if (chosen is ThornsChoice)
                {
                    await ChoosePower(choiceContext, 2);
                }
            }
        }

        public async Task ChoosePower(PlayerChoiceContext choiceContext, int choose, bool random = false)
        {
            int c = choose;
            if (random)
            {
                c = MiyabiFuncBase.RandomInt(1, 4, Owner);
            }

            switch (c)
            {
                case 1:
                    await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].BaseValue, Owner.Creature, null);
                    break;
                case 2:
                    await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars["Thorns"].BaseValue, Owner.Creature, null);
                    break;
                case 3:
                    foreach(var enemy in Owner.Creature.CombatState.HittableEnemies)
                    {
                        await PowerCmd.Apply<ShufaZhi>(choiceContext, enemy, DynamicVars["Shufa"].BaseValue, Owner.Creature, null);
                    }
                    break;
                default:
                    await PlayerCmd.GainEnergy(1, Owner);
                    break;
            }
        }

        public void SetThreshold(int threshold) => Threshold = threshold;
        public void ResetThreshold() => Threshold = 30;

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            ResetThreshold();
        }

    }
}
