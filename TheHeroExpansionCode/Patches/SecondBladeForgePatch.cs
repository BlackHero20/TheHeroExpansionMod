using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterForge))]
public static class SecondBladeForgePatch
{
    public static void Postfix(ICombatState combatState, decimal amount, Player forger, AbstractModel source)
    {
        foreach (TheSecondBlade card in forger.PlayerCombatState.AllCards.OfType<TheSecondBlade>())
        {
            if (card.IsDupe)
                continue;

            card.AddDamage(amount);
            ForgeCmd.PlayCombatRoomForgeVfx(forger, card);
        }
    }
}