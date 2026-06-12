using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(DefectCardPool))]
public class LootBox() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        LootBox lootBox = this;

        foreach (var generated in CardFactory.GetDistinctForCombat(
                     lootBox.Owner,
                     lootBox.Owner.Character.CardPool
                         .GetUnlockedCards(lootBox.Owner.UnlockState, lootBox.RunState.CardMultiplayerConstraint)
                         .Where(c => c.Rarity == CardRarity.Common),
                     lootBox.DynamicVars.Cards.IntValue,
                     lootBox.Owner.RunState.Rng.CombatCardGeneration))
        {
            generated.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, lootBox.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}