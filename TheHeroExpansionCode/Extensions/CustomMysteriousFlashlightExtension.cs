using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Extensions;

public static class CustomMysteriousFlashlightExtension
{
    internal static readonly Dictionary<ModelId, ModelId> CustomFlashlightCards = new();
 
    public static void AddFlashlightCardForCustomCharacter(this CardModel flashlightCard, CharacterModel characterModel)
    {
        CustomFlashlightCards[characterModel.Id] = flashlightCard.Id;
    }
}