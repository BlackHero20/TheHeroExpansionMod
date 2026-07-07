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
[Pool(typeof(IroncladCardPool))]
public class AllEndsInFlames() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        AllEndsInFlames allEndsInFlames = this;
        await CreatureCmd.TriggerAnim(allEndsInFlames.Owner.Creature, "Cast", allEndsInFlames.Owner.Character.CastAnimDelay);
        CardSelectorPrefs prefs = new CardSelectorPrefs(allEndsInFlames.SelectionScreenPrompt, 0, allEndsInFlames.DynamicVars.Cards.IntValue);
        IEnumerable<CardModel> cards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            (IReadOnlyList<CardModel>)PileType.Draw.GetPile(allEndsInFlames.Owner).Cards
                .OrderBy(c => c.Rarity)
                .ThenBy(c => c.Id)
                .ToList(),
            allEndsInFlames.Owner,
            prefs);
        foreach (CardModel card in cards)
            await CardCmd.Exhaust(choiceContext, card);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(2M);
    }
}