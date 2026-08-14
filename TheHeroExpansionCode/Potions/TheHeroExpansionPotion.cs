using BaseLib.Abstracts;
using BaseLib.Extensions;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Potions;
 
public abstract class TheHeroExpansionPotion : CustomPotionModel
{
    private static bool IsConfigEnabled(string className)
    {
        var prop = typeof(TheHeroExpansionConfig).GetProperty(className);
        return prop == null || (bool)prop.GetValue(null)!;
    }
 
    protected TheHeroExpansionPotion(string configName)
        : base(IsConfigEnabled(configName))
    {
    }
 
    protected TheHeroExpansionPotion() : base(true)
    {
    }
 
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
}