using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(DemonTongue), nameof(DemonTongue.AfterDamageReceived))]
public static class DemonTongue_AfterDamageReceived_Patch
{
    public static bool Prefix(DemonTongue __instance, ref Task __result)
    {
        if (__instance.Owner.Creature.HasPower<BetterDaysPower>())
        {
            __result = Task.CompletedTask;
            return false; 
        }

        return true;
    }
}