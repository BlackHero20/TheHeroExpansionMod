using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

public static class EnchantmentBlockContext
{
    public static ValueProp CurrentBlockProps;
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class InfectedOnPlayWrapperPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance)
    {
        if (__instance?.Owner == null) return;
        var pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Hand) return;
        var hand = pile.Cards;
        if (hand == null) return;

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] != __instance) continue;
            CardModel? left = i > 0 ? hand[i - 1] : null;
            CardModel? right = i < hand.Count - 1 ? hand[i + 1] : null;
            InfectedHelper.CardNeighborCache[__instance] = (left, right);
            return;
        }
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class InfectedCleanupPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardPlay cardPlay)
    {
        if (cardPlay?.Card != null)
            InfectedHelper.CardNeighborCache.Remove(cardPlay.Card);
    }
}

[HarmonyPatch(typeof(EnchantmentModel), "DynamicDescription", MethodType.Getter)]
public static class EnchantmentDynamicDescriptionPatch
{
    public static void Postfix(EnchantmentModel __instance, ref LocString __result)
    {
        __result.Add("InCombat", CombatManager.Instance.IsInProgress);
    }
}

[HarmonyPatch(typeof(EnchantmentModel), "DynamicExtraCardText", MethodType.Getter)]
public static class EnchantmentExtraCardTextPatch
{
    public static void Postfix(EnchantmentModel __instance, ref LocString __result)
    {
        if (__result == null) return;
        __result.Add("InCombat", CombatManager.Instance.IsInProgress);
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyBlock))]
public static class ModifyBlockPropsPatch
{
    public static void Prefix(ValueProp props)
    {
        EnchantmentBlockContext.CurrentBlockProps = props;
    }

    public static void Postfix()
    {
        EnchantmentBlockContext.CurrentBlockProps = default;
    }
}