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
public class Seek() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Event,
    TargetType.Self)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<DefectCardPool>();
    
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
        Seek seek = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(seek.SelectionScreenPrompt, seek.DynamicVars.Cards.IntValue);
        IEnumerable<CardModel> cards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            (IReadOnlyList<CardModel>)PileType.Draw.GetPile(seek.Owner).Cards
                .OrderBy(c => c.Rarity)
                .ThenBy(c => c.Id)
                .ToList(),
            seek.Owner,
            prefs);
        foreach (CardModel card in cards)
            await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1M);
    }
}