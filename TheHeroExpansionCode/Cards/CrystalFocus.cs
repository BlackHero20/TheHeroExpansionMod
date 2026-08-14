using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class CrystalFocus() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self, nameof(CrystalFocus))
{
    public override bool GainsBlock => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5M, ValueProp.Move),
        new IntVar("RetainBlock", 3M),
        new IntVar("BlockThreshold", 14M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CrystalFocus crystalFocus = this;
        await CreatureCmd.GainBlock(crystalFocus.Owner.Creature, crystalFocus.DynamicVars.Block, cardPlay);
    }
    
    public override async Task AfterBlockGained(
        Creature creature,
        decimal amount,
        ValueProp props,
        CardModel? cardSource)
    {
        CrystalFocus crystalFocus = this;
        if (cardSource != crystalFocus || creature != crystalFocus.Owner.Creature)
            return;
        if (amount < crystalFocus.DynamicVars["BlockThreshold"].BaseValue)
            return;

        ICombatState combatState = crystalFocus.CombatState;
        Rng rng = crystalFocus.Owner.RunState.Rng.CombatTargets;
        Creature? randomEnemy = rng.NextItem(combatState.HittableEnemies);
        if (randomEnemy == null)
            return;

        decimal blockDamage = crystalFocus.Owner.Creature.Block;

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            randomEnemy,
            blockDamage,
            ValueProp.Unpowered,
            crystalFocus.Owner.Creature);
    }

    public override Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        CrystalFocus crystalFocus = this;
        if (player != crystalFocus.Owner)
            return Task.CompletedTask;
        if (!retainedCards.Contains(crystalFocus))
            return Task.CompletedTask;

        crystalFocus.DynamicVars.Block.BaseValue += crystalFocus.DynamicVars["RetainBlock"].BaseValue;
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3M);
    }
}