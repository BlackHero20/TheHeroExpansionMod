using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Extensions;
public static class CustomBrokenBladeExtension
{
    internal static readonly Dictionary<ModelId, ModelId> CustomBladeCards = new();

    public static void AddBladeCardForCustomCharacter(this CardModel bladeCard, CharacterModel characterModel)
    {
        CustomBladeCards[characterModel.Id] = bladeCard.Id;
    }
}