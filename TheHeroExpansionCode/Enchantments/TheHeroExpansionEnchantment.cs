using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public class TheHeroExpansionEnchantment : CustomEnchantmentModel
{
    //Loads from TheHeroExpansion/images/powers/your_power.png
    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}