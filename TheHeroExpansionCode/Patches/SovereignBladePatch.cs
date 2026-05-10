using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(SovereignBlade))]
[HarmonyPatch("CanonicalVars", MethodType.Getter)]
public static class SovereignBladePatch
{
    public static void Postfix(ref IEnumerable<DynamicVar> __result)
    {
        __result = __result.Append(new DamageVar("EdgeOfDestinyDamage", 0M, ValueProp.Move));
    }
}