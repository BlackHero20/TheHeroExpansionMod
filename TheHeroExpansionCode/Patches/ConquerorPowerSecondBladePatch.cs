using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(ConquerorPower), nameof(ConquerorPower.ModifyDamageMultiplicative))]
public static class ConquerorPowerSecondBladePatch
{
    public static void Postfix(
        ConquerorPower __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay,
        ref decimal __result)
    {
        if (cardSource is TheSecondBlade && props.IsPoweredAttack() && target == __instance.Owner)
            __result = 2M;
    }
}