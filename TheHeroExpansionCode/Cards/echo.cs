using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(EventCardPool))]
public class Echo() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];
   
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Echo echo = this;
        var players = echo.CombatState?.PlayerCreatures.Select(c => c.Player).Distinct();
        if (players == null) return;

        var alliedDiscards = players
            .Where(p => p != echo.Owner)
            .SelectMany(p => PileType.Discard.GetPile(p).Cards)
            .OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id)
            .ToList();

        if (alliedDiscards.Count == 0) return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(
            echo.SelectionScreenPrompt,
            echo.DynamicVars.Cards.IntValue);

        CardModel? chosen = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            (IReadOnlyList<CardModel>)alliedDiscards,
            echo.Owner,
            prefs)).FirstOrDefault();

        if (chosen == null) return;
        await CardPileCmd.Add(chosen, PileType.Draw, CardPilePosition.Top, echo);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}