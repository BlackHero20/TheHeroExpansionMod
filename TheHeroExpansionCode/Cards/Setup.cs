using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class Setup() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self, nameof(Setup))
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Setup setup = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(setup.SelectionScreenPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, setup.Owner, prefs,
            c => !c.EnergyCost.CostsX, setup)).FirstOrDefault();
        if (card == null)
            return;
        
        decimal currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.Local);
        if (currentCost > 0M)
            card.EnergyCost.AddUntilPlayed((int) -currentCost);

        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);

        if (setup.IsUpgraded)
            CardCmd.Upgrade(card);
    }
}