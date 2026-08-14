using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class SummonForthSecondBladePatch
{
    public static async void Postfix(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not SummonForth summonForth)
            return;

        foreach (TheSecondBlade card in summonForth.Owner.PlayerCombatState.AllCards.OfType<TheSecondBlade>())
        {
            if (card.Pile?.Type == PileType.Hand)
                continue;

            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
}