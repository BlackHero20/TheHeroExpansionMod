using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class TheScorchedBlade() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Event,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<IroncladCardPool>();
    
    private readonly List<CardModel> _exhaustedByThisCard = [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    
    public override Task BeforeCombatStartLate()
    {
        _exhaustedByThisCard.Clear();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheScorchedBlade theScorchedBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
 
        await DamageCmd.Attack(theScorchedBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theScorchedBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        foreach (CardModel previouslyExhausted in theScorchedBlade._exhaustedByThisCard.ToList())
        {
            await CardCmd.AutoPlay(choiceContext, previouslyExhausted, null);
            if (previouslyExhausted.Pile?.Type != PileType.Exhaust)
                await CardPileCmd.Add(previouslyExhausted, PileType.Exhaust, CardPilePosition.Random, null, skipVisuals: true);
        }
 
        CardModel? selected;
        if (theScorchedBlade.IsUpgraded)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(theScorchedBlade.SelectionScreenPrompt, 1);
            selected = (await CardSelectCmd.FromHand(choiceContext, theScorchedBlade.Owner, prefs,
                c => c != theScorchedBlade && c.Type == CardType.Skill, theScorchedBlade)).FirstOrDefault();
        }
        else
        {
            CardPile pile = PileType.Hand.GetPile(theScorchedBlade.Owner);
            selected = theScorchedBlade.Owner.RunState.Rng.CombatCardSelection.NextItem(
                pile.Cards.Where(c => c != theScorchedBlade && c.Type == CardType.Skill));
        }
 
        if (selected == null)
            return;
 
        theScorchedBlade._exhaustedByThisCard.Add(selected);
        await CardCmd.Exhaust(choiceContext, selected);
    }
 
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3M);
    }
}