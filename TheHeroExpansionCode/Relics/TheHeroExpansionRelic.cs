using BaseLib.Abstracts;
using BaseLib.Extensions;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
using Godot;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Relics;
 
public abstract class TheHeroExpansionRelic : CustomRelicModel
{
    private static bool IsConfigEnabled(string className)
    {
        var prop = typeof(TheHeroExpansionConfig).GetProperty(className);
        return prop == null || (bool)prop.GetValue(null)!;
    }
 
    protected TheHeroExpansionRelic(string configName)
        : base(IsConfigEnabled(configName))
    {
    }
 
    protected TheHeroExpansionRelic() : base(true)
    {
    }
 
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }
 
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }
 
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}