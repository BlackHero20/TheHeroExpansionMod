using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

public abstract class VakuuCard(int cost, CardType type, TargetType targetType)
    : TheHeroExpansionCard(cost, type, CardRarity.Ancient, targetType)
{
    protected override bool ShouldGlowRedInternal => true;
    public override int MaxUpgradeLevel => 1;

    private bool _isAutoPlaying = false;

    private static readonly AsyncLocal<PlayerChoiceContext?> AmbientChoiceContext = new();

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != this.Owner) return true;
        CardPile pile = this.Pile;
        return (pile != null ? (pile.Type != PileType.Hand ? 1 : 0) : 1) != 0
               || card is VakuuCard
               || autoPlayType != AutoPlayType.None;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        VakuuCard vakuuCard = this;
        if (card != vakuuCard) return;
        if (_isAutoPlaying) return;
        if (card.Pile?.Type == PileType.Hand) return;
        if (card.Pile?.Type == PileType.Play || card.Pile?.Type == PileType.Exhaust) return;
        if (vakuuCard.Owner == null) return;

        var combatState = vakuuCard.Owner.Creature.CombatState;
        if (combatState == null || CombatManager.Instance.IsOverOrEnding) return;

        _isAutoPlaying = true;
        try
        {
            PlayerChoiceContext choiceContext =
                AmbientChoiceContext.Value ?? (PlayerChoiceContext)new ThrowingPlayerChoiceContext();

            await CardCmd.AutoPlay(choiceContext, card, null, AutoPlayType.None);
        }
        finally
        {
            _isAutoPlaying = false;
        }
    }

    protected LocString GetRandomVakuuTaunt()
    {
        int roll = this.Owner.RunState.Rng.CombatTargets.NextInt(0, 5);
        return new LocString("ancients", $"VAKUU.taunt.{roll}");
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand<T>(
        Player owner,
        int count,
        ICombatState combatState,
        PlayerChoiceContext? choiceContext = null) where T : VakuuCard
    {
        if (count == 0 || CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();

        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < count; ++i)
            cards.Add(combatState.CreateCard<T>(owner));

        await WithAmbientChoiceContext(choiceContext,
            async () => { await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, owner); });

        return cards;
    }
    
    public static async Task<TResult> WithAmbientChoiceContext<TResult>(
        PlayerChoiceContext? choiceContext, Func<Task<TResult>> action)
    {
        var previous = AmbientChoiceContext.Value;
        AmbientChoiceContext.Value = choiceContext;
        try
        {
            return await action();
        }
        finally
        {
            AmbientChoiceContext.Value = previous;
        }
    }
    
    private static async Task WithAmbientChoiceContext(
        PlayerChoiceContext? choiceContext, Func<Task> action)
    {
        var previous = AmbientChoiceContext.Value;
        AmbientChoiceContext.Value = choiceContext;
        try
        {
            await action();
        }
        finally
        {
            AmbientChoiceContext.Value = previous;
        }
    }
}