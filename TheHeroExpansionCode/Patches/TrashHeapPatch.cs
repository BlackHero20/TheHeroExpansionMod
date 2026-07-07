using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(TrashHeap), "Cards", MethodType.Getter)]
public static class TrashHeapPatch
{
    public static void Postfix(ref CardModel[] __result)
    {
        var list = __result.ToList();
        list.Add(ModelDb.Card<MasterfulStab>());
        __result = list.ToArray();
    }
}