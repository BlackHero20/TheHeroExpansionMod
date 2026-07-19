using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.GetUnlockedCards))]
public static class DeterministicCardPoolOrderPatch
{
    public static void Postfix(ref IEnumerable<CardModel> __result)
    {
        __result = __result.OrderBy(c => c.Id).ToList();
    }
}
 
