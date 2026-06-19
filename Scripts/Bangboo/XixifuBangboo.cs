using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo
{
    internal class XixifuBangboo : MiyabiBangbooBase
    {
        protected override string VisualsPath => "res://scenes/bangboo/xixifu.tscn";

        public override async Task OnSummon(PlayerChoiceContext choiceContext, Player owner, MinionSummonOptions options)
        {
            await base.OnSummon(choiceContext, owner, options);

            await PowerCmd.Apply<XixifuAct>(new ThrowingPlayerChoiceContext(), this.Creature, 1m, owner.Creature, options.Source);
        }
    }

    internal class XixifuAct : MiyabiBangbooActBase
    {
        public override TargetType TargetType => TargetType.None;
        public override string BigIconPath => "res://images/bangboo/relicMode/xixifuRelic.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("MAXUSE", MAXUSE),
            new DynamicVar("Used", 0),
            new DamageVar(8m, ValueProp.Move),
            new DynamicVar("Block", 6m),
            new DynamicVar("Gold", 10m),
        ];

        // Xixifu doesn't cost energy — once per turn free use
        protected override bool CanPayCost => true;

        //protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
        //{
        //    DynamicVars["MAXUSE"].BaseValue = MAXUSE;
        //    DynamicVars["Used"].BaseValue = UsedCount;

        //    if (UsedCount >= MAXUSE && isFree < 1) return;

        //    bool cardRemoved = await DoCardSelectAndRemove(choiceContext);

        //    if (!cardRemoved) return; // No card removed — don't consume use

        //    // Consume use
        //    if (isFree < 1)
        //    {
        //        UsedCount++;
        //        DynamicVars["Used"].BaseValue = UsedCount;
        //    }
        //    isFree--;
        //    if (isFree < 0) isFree = 0;

        //    // Trigger random bonus effect
        //    await TriggerRandomEffect(choiceContext);
        //}

        /// <summary>
        /// Let player select up to 1 card from hand and exhaust it.
        /// Returns true if a card was successfully removed.
        /// </summary>
        private async Task<bool> DoCardSelectAndRemove(PlayerChoiceContext choiceContext)
        {
            var handCards = Owner.PetOwner.PlayerCombatState.Hand.Cards.ToList();
            if (handCards.Count == 0) return false;

            CardSelectorPrefs prefs = new CardSelectorPrefs(
                CardSelectorPrefs.RemoveSelectionPrompt, 0, 1);
            List<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext, handCards, Owner.PetOwner, prefs)).ToList();

            if (selected.Count == 0 || selected[0] == null) return false;

            await CardCmd.Exhaust(choiceContext, selected[0]);
            return true;
        }

        /// <summary>
        /// Randomly trigger one bonus effect after a card is removed.
        /// Effects: ① Gain Gold  ② Damage all enemies  ③ Gain Block
        /// </summary>
        private async Task TriggerRandomEffect(PlayerChoiceContext choiceContext)
        {
            int roll = MiyabiFuncBase.RandomInt(0, 5, Owner.PetOwner);
            switch (roll)
            {
                case 0: // Gain gold
                    await PlayerCmd.GainGold(
                        DynamicVars["Gold"].IntValue, Owner.PetOwner);
                    TalkCmd.Play(MiyabiFuncBase.GetForMonsterString("XIXIFU_GET_MONEY"), Owner, VfxColor.Blue);
                    break;
                case 1: // Deal damage to all enemies
                    var enemies = Owner.CombatState.Enemies
                        .Where(e => e != null && e.IsAlive)
                        .ToList();
                    if (enemies.Count > 0)
                        await CreatureCmd.Damage(
                            choiceContext, enemies, DynamicVars.Damage, Owner);
                    TalkCmd.Play(MiyabiFuncBase.GetForMonsterString("XIXIFU_DAMAGE"), Owner, VfxColor.Blue);
                    break;
                case 2: // Gain block
                    await CreatureCmd.GainBlock(
                        Owner.PetOwner.Creature,
                        DynamicVars["Block"].BaseValue,
                        ValueProp.Unpowered, null);
                    TalkCmd.Play(MiyabiFuncBase.GetForMonsterString("XIXIFU_BLOCK"), Owner, VfxColor.Blue);
                    break;
                default:
                    TalkCmd.Play(MiyabiFuncBase.GetForMonsterString("XIXIFU_NOMOVE"), Owner, VfxColor.Blue);
                    break;
            }
        }

        // No energy cost for Xixifu
        public override async Task ActCost()
        {
            // Intentionally empty — Xixifu has no cost.
            // The UsedCount increment is handled in OnAct override.
            UsedCount++;
            DynamicVars["Used"].BaseValue = UsedCount;
        }

        // ActEffect is not used directly since OnAct is fully overridden,
        // but keep it as a no-op for compatibility with OnCardActivate etc.
        public override async Task ActEffect(PlayerChoiceContext choiceContext, Creature? target)
        {
            bool cardRemoved = await DoCardSelectAndRemove(choiceContext);
            if (cardRemoved) await TriggerRandomEffect(choiceContext);
        }
    }
}
