using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class DiscardedContract() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DiscardedContract discardedContract = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(discardedContract.SelectionScreenPrompt, discardedContract.DynamicVars.Cards.IntValue);
        IEnumerable<CardModel> cards;
        if (discardedContract.IsUpgraded)
        {
            cards = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                (IReadOnlyList<CardModel>)PileType.Draw.GetPile(discardedContract.Owner).Cards
                    .OrderBy(c => c.Rarity)
                    .ThenBy(c => c.Id)
                    .ToList(),
                discardedContract.Owner,
                prefs);
        }
        else
        {
            cards = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                (IReadOnlyList<CardModel>)PileType.Discard.GetPile(discardedContract.Owner).Cards
                    .OrderBy(c => c.Rarity)
                    .ThenBy(c => c.Id)
                    .ToList(),
                discardedContract.Owner,
                prefs);
        }
        foreach (CardModel card in cards)
            await CardPileCmd.Add(card, PileType.Hand);
    }
}