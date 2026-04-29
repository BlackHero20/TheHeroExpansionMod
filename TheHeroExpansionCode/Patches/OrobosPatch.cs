using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode;

[HarmonyPatch(typeof(Orobas), "get_OptionPool2")]
public static class OrobasPatch
{
    public static void Postfix(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        var relic = ModelDb.Relic<TestRelic>().ToMutable(); //change this to whatever relic I want
        
        var method = typeof(AncientEventModel).GetMethod("RelicOption",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null, new[] { typeof(RelicModel), typeof(string), typeof(string) }, null);

        var option = (EventOption)method.Invoke(__instance, new object[] { relic, "INITIAL", null });
        var list = __result.ToList();
        list.Add(option);
        __result = list;
    }
}