using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(SwordSagePower), nameof(SwordSagePower.AfterPowerAmountChanged))]
public static class SwordSageSecondBladeAmountChangedPatch
{
    public static void Postfix(
        SwordSagePower __instance,
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not SwordSagePower || power.Owner != __instance.Owner)
            return;

        Player? player = __instance.Owner.Player;
        if (player?.PlayerCombatState == null)
            return;

        foreach (TheSecondBlade card in player.PlayerCombatState.AllCards.OfType<TheSecondBlade>())
        {
            if (card.Owner != player)
                continue;

            card.BaseReplayCount += (int)amount;
        }
    }
}

[HarmonyPatch(typeof(SwordSagePower), nameof(SwordSagePower.AfterCardEnteredCombat))]
public static class SwordSageSecondBladeEnteredCombatPatch
{
    public static void Postfix(SwordSagePower __instance, CardModel card)
    {
        if (card.IsClone)
            return;
        if (card is not TheSecondBlade secondBlade)
            return;
        if (secondBlade.Owner != __instance.Owner.Player)
            return;

        secondBlade.BaseReplayCount += (int)__instance.Amount;
    }
}

[HarmonyPatch(typeof(SwordSagePower), nameof(SwordSagePower.AfterRemoved))]
public static class SwordSageSecondBladeRemovedPatch
{
    public static void Postfix(SwordSagePower __instance, Creature oldOwner)
    {
        Player? player = oldOwner.Player;
        if (player?.PlayerCombatState == null)
            return;

        foreach (TheSecondBlade card in player.PlayerCombatState.AllCards.OfType<TheSecondBlade>())
        {
            if (card.Owner != player)
                continue;

            card.BaseReplayCount -= (int)__instance.Amount;
        }
    }
}