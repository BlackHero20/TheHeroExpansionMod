using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(DieForYouPower), nameof(DieForYouPower.ModifyUnblockedDamageTarget))]
public static class DieForYouPatch
{
    public static void Postfix(DieForYouPower __instance, ref Creature __result, Creature target)
    {
        if (__result == __instance.Owner &&
            __instance.Owner.PetOwner?.Creature.HasPower<GotYourBackPower>() == true)
        {
            __result = target;
        }
    }
}