using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

public abstract class VakuuCard(int cost, CardType type, TargetType targetType)
    : TheHeroExpansionCard(cost, type, CardRarity.Ancient, targetType)
{
    protected override bool ShouldGlowRedInternal => true;

    public override int MaxUpgradeLevel => 1;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != this.Owner) return true;
        CardPile pile = this.Pile;
        return (pile != null ? (pile.Type != PileType.Hand ? 1 : 0) : 1) != 0
               || card.GetType() == this.GetType()
               || autoPlayType != AutoPlayType.None;
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand<T>(
        Player owner,
        int count,
        ICombatState combatState) where T : VakuuCard
    {
        if (count == 0 || CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();
        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < count; ++i)
            cards.Add(combatState.CreateCard<T>(owner));
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, owner);
        return cards;
    }
}