using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;


namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class Exhume() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Event,
    TargetType.Self)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<IroncladCardPool>();
    
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
        Exhume exhume = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(exhume.SelectionScreenPrompt, exhume.DynamicVars.Cards.IntValue);
        CardModel card = (await CardSelectCmd.FromSimpleGrid(choiceContext, (IReadOnlyList<CardModel>) PileType.Exhaust.GetPile(exhume.Owner).Cards.OrderBy<CardModel, CardRarity>((Func<CardModel, CardRarity>) (c => c.Rarity)).ThenBy<CardModel, ModelId>((Func<CardModel, ModelId>) (c => c.Id)).ToList<CardModel>(), exhume.Owner, prefs)).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}