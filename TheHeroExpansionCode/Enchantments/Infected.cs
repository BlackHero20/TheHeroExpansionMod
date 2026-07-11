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
using TheHeroExpansion.TheHeroExpansionCode.Patches;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Infected : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("InfectedPercent", 0M)
    ];

    public override void RecalculateValues()
    {
        if (Card == null)
            return;

        var count = GetInfectedCount(Card);
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

        if (Card?.Owner?.PlayerCombatState == null)
            return 1M;

        int count = Card.Owner.PlayerCombatState.AllCards.Count(c => c.Enchantment is Infected);
        decimal percent = 5m * count;
        return 1m + (percent / 100m);
    }

    public override Decimal EnchantBlockMultiplicative(Decimal originalBlock)
    {
        if (Card?.Owner?.PlayerCombatState == null)
            return 1M;

        var props = EnchantmentBlockContext.CurrentBlockProps;
        if (!props.IsPoweredCardOrMonsterMoveBlock())
            return 1M;

        int count = Card.Owner.PlayerCombatState.AllCards.Count(c => c.Enchantment is Infected);
        decimal percent = 5m * count;
        return 1m + (percent / 100m);
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay?.Card == null) return;
        if (!InfectedHelper.CardNeighborCache.TryGetValue(cardPlay.Card, out var neighbors)) return;

        var (leftCard, rightCard) = neighbors;

        if (leftCard != null && leftCard.Enchantment == null &&
            leftCard.Type is CardType.Attack or CardType.Skill or CardType.Power)
            CardCmd.Enchant<Infected>(leftCard, 1);

        if (rightCard != null && rightCard.Enchantment == null &&
            rightCard.Type is CardType.Attack or CardType.Skill or CardType.Power)
            CardCmd.Enchant<Infected>(rightCard, 1);
    }
}