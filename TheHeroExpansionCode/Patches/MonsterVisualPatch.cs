using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

public static class MonsterVisualPatch
{
    // Fixes _Ready() for programmatically created NCreatureVisuals.
    // Required for all sprite-based custom monsters using CreateCustomVisuals().
    // Add once — applies to every custom monster automatically.
    [HarmonyPatch(typeof(NCreatureVisuals), "_Ready")]
    public static class NCreatureVisualsReadyPatch
    {
        public static bool Prefix(NCreatureVisuals __instance)
        {
            if (__instance.GetNodeOrNull<Node2D>("%Visuals") != null)
                return true;

            var body = __instance.GetNodeOrNull<Node2D>("Visuals");
            if (body == null)
                return true;

            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var type = typeof(NCreatureVisuals);

            type.GetField("_body", flags)?.SetValue(__instance, body);
            type.GetProperty("Bounds", flags)?.SetValue(__instance,
                __instance.GetNodeOrNull<Control>("Bounds") ?? new Control());
            var intentPos = __instance.GetNodeOrNull<Marker2D>("IntentPos") ?? new Marker2D();
            type.GetProperty("IntentPosition", flags)?.SetValue(__instance, intentPos);
            type.GetProperty("VfxSpawnPosition", flags)?.SetValue(__instance,
                __instance.GetNodeOrNull<Marker2D>("CenterPos") ?? new Marker2D());
            type.GetProperty("OrbPosition", flags)?.SetValue(__instance,
                __instance.GetNodeOrNull<Marker2D>("OrbPos") ?? intentPos);

            // Set owner so %Name lookups work from NCreature
            foreach (var child in __instance.GetChildren())
                child.Owner = __instance;

            return false;
        }
    }

    // Skips Spine-specific scale/hue logic for sprite-based monsters
    [HarmonyPatch(typeof(NCreatureVisuals), nameof(NCreatureVisuals.SetScaleAndHue))]
    public static class MonsterSetScaleAndHuePatch
    {
        public static bool Prefix(NCreatureVisuals __instance)
        {
            if (__instance.GetNodeOrNull<Node2D>("Visuals") != null &&
                __instance.GetNodeOrNull<Node2D>("%Visuals") == null)
                return false;
            return true;
        }
    }
}