using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode;

[HarmonyPatch(typeof(Tezcatara), "get_OptionPool1")]
public static class TezcaPatch
{
    public static void Postfix(Tezcatara __instance, ref IEnumerable<EventOption> __result)
    {
        if (!TheHeroExpansionConfig.NostalgicSmores)
            return;
        
        var relic = ModelDb.Relic<NostalgicSmores>().ToMutable();

        var method = typeof(AncientEventModel).GetMethod("RelicOption",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null, new[] { typeof(RelicModel), typeof(string) }, null);

        var option = (EventOption)method.Invoke(__instance, new object[] { relic, "INITIAL" });
        var list = __result.ToList();
        list.Add(option);
        __result = list;
    }
}