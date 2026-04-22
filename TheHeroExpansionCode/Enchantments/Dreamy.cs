using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Dreamy : TheHeroExpansionEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) =>
        cardType == CardType.Attack || cardType == CardType.Skill;

    public override bool IsStackable => false;
    
    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Dreamy enchantment = this;
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                enchantment.Card.CreateClone(), PileType.Discard, true), 2.2f);
    }
}