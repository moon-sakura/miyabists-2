using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Miyabists2.Scripts.Powers;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;

namespace Miyabists2.Scripts.Cards
{
    internal class ChunLin : MiyabiAttackCardBase
    {
        //public override string PortraitPath => $"res://images/cards/feng_hua.png";

        public ChunLin() : base(2, CardRarity.Rare, TargetType.AllEnemies, true) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(16, ValueProp.Move),
            new DynamicVar(DazeVarName, 4)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //群体伤害
            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(hittableEnemy));
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this).TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);

            //选择一张伙伴卡加入手卡
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
            List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
                                       where c.CanonicalKeywords.Contains(MiyabiKeywords.Friends)
                                       orderby c.Rarity, c.Id
                                       select c).ToList();
            if(cardsIn.Count != 0)
            {
                CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs)).FirstOrDefault();
                if (cardModel != null)
                {
                    cardModel.AddKeyword(CardKeyword.Retain);
                    await CardPileCmd.Add(cardModel, PileType.Hand);
                }
            }

        }


        public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //本回合击破过减费
            bool hasBreak = false;
            foreach (Creature Enemy in base.CombatState.Enemies)
            {
                if(Enemy.HasPower<BreakPower>()) hasBreak = true;
            }
            if(hasBreak) ReduceCostBy(2);
        }


        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4);
            if (base.DynamicVars.TryGetValue(DazeVarName, out DynamicVar v)) v.UpgradeValueBy(2);
        }
        private void ReduceCostBy(int amount)
        {
            base.EnergyCost.AddThisTurn(-amount);
        }
    }
}
