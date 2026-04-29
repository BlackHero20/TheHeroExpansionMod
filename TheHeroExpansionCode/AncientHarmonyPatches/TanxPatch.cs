using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode;

[HarmonyPatch(typeof(Tanx), "get_BaseOptionPool")]
public static class TanxPatch
{
    public static void Postfix(Tanx __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance.Owner == null)
            return;

        var relic = ModelDb.Relic<TestRelic>().ToMutable(); //change this to whatever relic I want
        relic.Owner = __instance.Owner;

        // Use reflection to call the protected RelicOption method
        var method = typeof(AncientEventModel).GetMethod("RelicOption",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null, new[] { typeof(RelicModel), typeof(string), typeof(string) }, null);

        var option = (EventOption)method.Invoke(__instance, new object[] { relic, "INITIAL", null });
        __result = __result.Append(option);
    }
}