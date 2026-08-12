using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode;
 
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "TheHeroExpansion";
 
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);
 
    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new TheHeroExpansionConfig());
        
        Harmony harmony = new(ModId);
 
        harmony.PatchAll();
    }
}