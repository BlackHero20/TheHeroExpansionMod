using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class TheBrokenBlade() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    private int _playsRemaining = 5;

    // Persists across combats within the run, same pattern as Dowsing's
    // RoomsEntered / TheScythe's CurrentDamage.
    [SavedProperty]
    public int PlaysRemaining
    {
        get => _playsRemaining;
        set
        {
            AssertMutable();
            _playsRemaining = value;
            DynamicVars["PlaysRemaining"].BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1M, ValueProp.Move),
        new IntVar("PlaysRemaining", _playsRemaining)
    ];

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("BladeName", GetBladeName());
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (this.IsCanonical || this.Owner == null)
                return Array.Empty<IHoverTip>();

            CardModel? card = GetBladeInstanceForCharacter(this.Owner.Character);
            return card == null ? Array.Empty<IHoverTip>() : new[] { HoverTipFactory.FromCard(card) };
        }
    }

    // Compendium/no-owner view shows the generic placeholder text; an actual
    // owned instance shows the real blade's localized title.
    private string GetBladeName()
    {
        if (this.IsCanonical || this.Owner == null)
            return "character specific blade";

        CardModel? card = GetBladeInstanceForCharacter(this.Owner.Character);
        return card?.Title ?? "character specific blade";
    }

    private static CardModel? GetBladeInstanceForCharacter(CharacterModel character) => character switch
    {
        Ironclad => ModelDb.Card<TheScorchedBlade>(),
        Silent => ModelDb.Card<TheTaintedBlade>(),
        Regent => ModelDb.Card<TheSecondBlade>(),
        Necrobinder => ModelDb.Card<TheMarrowBlade>(),
        Defect => ModelDb.Card<TheSavageBlade>(),
        _ => ModelDb.Card<TheFinishedBlade>()
    };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheBrokenBlade theBrokenBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(theBrokenBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theBrokenBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        theBrokenBlade.PlaysRemaining--;

        // The card played in combat is a separate instance from its permanent
        // deck copy - same dual-update TheScythe does for its own persistent
        // stat via DeckVersion, otherwise the change never survives past this
        // combat.
        TheBrokenBlade? deckVersion = theBrokenBlade.DeckVersion as TheBrokenBlade;
        if (deckVersion != null)
            deckVersion.PlaysRemaining = theBrokenBlade.PlaysRemaining;

        if (theBrokenBlade.PlaysRemaining > 0)
            return;

        // Whether the transformed result comes out upgraded depends on whether
        // THIS card (Broken Blade) was upgraded - not always-upgraded.
        bool shouldUpgradeResult = theBrokenBlade.IsUpgraded;

        // Transform the in-combat instance (temporary, for this fight's visuals/
        // mechanics) AND the deck version (permanent, so it sticks for the rest
        // of the run) - same reasoning as the DeckVersion.PlaysRemaining sync
        // above. TransformTo<T> needs a compile-time type argument, so this is
        // an explicit branch per character rather than something driven off the
        // GetBladeInstanceForCharacter switch used for the hover tip.
        if (theBrokenBlade.Owner.Character is Ironclad)
        {
            await TransformBothVersions<TheScorchedBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
        else if (theBrokenBlade.Owner.Character is Silent)
        {
            await TransformBothVersions<TheTaintedBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
        else if (theBrokenBlade.Owner.Character is Regent)
        {
            await TransformBothVersions<TheSecondBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
        else if (theBrokenBlade.Owner.Character is Necrobinder)
        {
            await TransformBothVersions<TheMarrowBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
        else if (theBrokenBlade.Owner.Character is Defect)
        {
            await TransformBothVersions<TheSavageBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
        else
        {
            await TransformBothVersions<TheFinishedBlade>(theBrokenBlade, deckVersion, shouldUpgradeResult);
        }
    }

    private static async Task TransformBothVersions<T>(
        TheBrokenBlade combatInstance,
        TheBrokenBlade? deckVersion,
        bool shouldUpgradeResult)
        where T : CardModel
    {
        // Combat instance - handles this fight's visuals/mechanics.
        CardPileAddResult? combatResult = await CardCmd.TransformTo<T>(combatInstance);
        if (combatResult?.cardAdded != null)
        {
            CardModel transformedCombatCard = combatResult.Value.cardAdded;
            if (shouldUpgradeResult)
                CardCmd.Upgrade(transformedCombatCard);

            // CardCmd.Transform re-inserts the replacement into whatever pile the
            // original was in (Play, since we're mid-OnPlay) - normal card-play
            // cleanup only ever looks at the OLD card reference (which Transform
            // already removed from its pile), so nothing else ever moves the new
            // card out of Play. Do it ourselves.
            if (transformedCombatCard.Pile?.Type == PileType.Play)
                await CardPileCmd.Add(transformedCombatCard, PileType.Discard);
        }

        // Deck version - makes the transform stick for the rest of the run,
        // same reasoning as Dowsing transforming while sitting in the Deck pile.
        if (deckVersion == null)
            return;

        CardPileAddResult? deckResult = await CardCmd.TransformTo<T>(deckVersion);
        if (shouldUpgradeResult && deckResult?.cardAdded != null)
            CardCmd.Upgrade(deckResult.Value.cardAdded);
    }
}