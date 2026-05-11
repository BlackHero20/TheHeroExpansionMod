using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard.AnimMultiCardPlay))]
public static class BouncingBladePatch
{
    public static bool Prefix(NCard __instance, ref Task __result)
    {
        if (__instance.Model is BouncingBlade)
        {
            __result = Task.CompletedTask;
            return false;
        }
        return true;
    }
}