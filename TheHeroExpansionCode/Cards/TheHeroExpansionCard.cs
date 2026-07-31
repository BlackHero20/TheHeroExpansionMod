using BaseLib.Abstracts;
using BaseLib.Extensions;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
 
public abstract class TheHeroExpansionCard : CustomCardModel
{
    private static bool IsConfigEnabled(string className)
    {
        var prop = typeof(TheHeroExpansionConfig).GetProperty(className);
        return prop == null || (bool)prop.GetValue(null)!;
    }
 
    protected TheHeroExpansionCard(int cost, CardType type, CardRarity rarity, TargetType target,
        string configName)
        : base(cost, type, rarity, target, true, IsConfigEnabled(configName))
    {
    }
 
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}