using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments
{
    /// <summary>
    /// Captures the card index BEFORE it leaves the hand.
    /// </summary>
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
    public static class InfectedOnPlayWrapperPatch
    {
        [HarmonyPrefix]
        public static void Prefix(CardModel __instance)
        {
            if (__instance?.Owner == null)
                return;

            var pile = __instance.Pile;
            if (pile == null || pile.Type != PileType.Hand)
                return;

            var hand = pile.Cards;
            if (hand == null)
                return;

            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i] == __instance)
                {
                    InfectedHelper.CardIndexCache[__instance] = i;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Cleanup after the card is played
    /// </summary>
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
    public static class InfectedCleanupPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardPlay cardPlay)
        {
            if (cardPlay?.Card != null)
            {
                InfectedHelper.CardIndexCache.Remove(cardPlay.Card);
            }
        }
    }
}