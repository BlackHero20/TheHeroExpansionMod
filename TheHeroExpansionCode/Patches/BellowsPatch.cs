using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;
//this patch exists to make sure Bellows cant upgrade the vakuu cards, as many have very negative effects when upgraded
[HarmonyPatch(typeof(Bellows), nameof(Bellows.AfterPlayerTurnStart))]
public static class BellowsPatch
{
    public static bool Prefix(Bellows __instance, Player player, ref Task __result)
    {
        if (player != __instance.Owner || player.Creature.CombatState.RoundNumber > 1)
            return true;

        __instance.Flash();
        var cards = PileType.Hand.GetPile(__instance.Owner).Cards
            .Where(c => c is not VakuusPrank && c is not LunchBreak && c is not DemonicSurge);
        CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        __result = Task.CompletedTask;
        return false;
    }
}