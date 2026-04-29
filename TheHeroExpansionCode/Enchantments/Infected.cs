using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Infected : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;

    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }

    // ✅ REAL dynamic variable (NOT CalculatedVar)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("InfectedPercent", 0M)
    ];

    public override void RecalculateValues()
    {
        if (Card == null)
            return;

        var count = GetInfectedCount(Card);

        // 5% per infected card
        this.DynamicVars["InfectedPercent"].BaseValue = 5m * count;
    }

    private static int GetInfectedCount(CardModel card)
    {
        var owner = card.Owner;
        if (owner?.PlayerCombatState == null)
            return 0;

        return owner.PlayerCombatState.AllCards.Count(c =>
            c.Enchantment is Infected);
    }

    public override Decimal EnchantDamageMultiplicative(Decimal originalDamage, ValueProp props)
    {
        if (!props.IsPoweredAttack())
            return 1M;

        if (Card == null)
            return 1M;

        var percent = this.DynamicVars["InfectedPercent"].BaseValue;
        return 1m + (percent / 100m);
    }

    public override Decimal EnchantBlockMultiplicative(Decimal originalBlock, ValueProp props)
    {
        if (!props.IsPoweredAttack())
            return 1M;

        if (Card == null)
            return 1M;

        var percent = this.DynamicVars["InfectedPercent"].BaseValue;
        return 1m + (percent / 100m);
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card == null)
            return;

        var card = cardPlay.Card;

        if (!InfectedHelper.CardIndexCache.TryGetValue(card, out var cardIndex))
            return;

        var owner = card.Owner;
        var hand = PileType.Hand.GetPile(owner).Cards.ToList();

        GD.Print($"[OnPlay] cached index = {cardIndex}, handCount = {hand.Count}");

        if (cardIndex - 1 >= 0)
        {
            var leftCard = hand[cardIndex - 1];
            if (leftCard.Enchantment == null && (leftCard.Type == CardType.Attack || leftCard.Type == CardType.Skill || leftCard.Type == CardType.Power))
                CardCmd.Enchant<Infected>(leftCard, 1);
        }

        if (cardIndex >= 0 && cardIndex < hand.Count)
        {
            var rightCard = hand[cardIndex];
            if (rightCard.Enchantment == null && (rightCard.Type == CardType.Attack || rightCard.Type == CardType.Skill || rightCard.Type == CardType.Power))
                CardCmd.Enchant<Infected>(rightCard, 1);
        }
    }
}