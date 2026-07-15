using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Dreamy : TheHeroExpansionEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) =>
        cardType == CardType.Attack || cardType == CardType.Skill;

    public override bool IsStackable => false;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Dreamy enchantment = this;

        CardPileAddResult addResult = await VakuuCard.WithAmbientChoiceContext(choiceContext,
            () => CardPileCmd.AddGeneratedCardToCombat(
                enchantment.Card.CreateClone(), PileType.Discard, enchantment.Card.Owner));

        CardCmd.PreviewCardPileAdd(addResult, 2.2f);
    }
}