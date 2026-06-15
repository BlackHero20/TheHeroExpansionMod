using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

public static class DarkMagicianPatch
{
    [HarmonyPatch(typeof(OrbCmd), nameof(OrbCmd.Channel),
        new[] { typeof(PlayerChoiceContext), typeof(OrbModel), typeof(Player) })]
    public static class OrbChannelPatch
    {
        public static void Postfix(
            ref Task __result,
            PlayerChoiceContext choiceContext,
            OrbModel orb,
            Player player)
        {
            if (player.Creature.GetPower<DarkMagicianPower>() == null) return;
            if (orb is not LightningOrb) return;

            __result = __result
                .ContinueWith(_ => OrbCmd.Channel<DarkOrb>(choiceContext, player))
                .Unwrap();
        }
    }
}