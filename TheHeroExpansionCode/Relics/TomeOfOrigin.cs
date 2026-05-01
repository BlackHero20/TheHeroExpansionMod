using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;
[Pool(typeof(EventRelicPool))]
public class TomeOfOrigin() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        TomeOfOrigin tomeOfOrigin = this;
        if (player != tomeOfOrigin.Owner)
            return;

        // Get card pools from other characters
        List<CardPoolModel> otherPools = tomeOfOrigin.Owner.UnlockState.CharacterCardPools.ToList();
        if (otherPools.Count > 1)
            otherPools.Remove(tomeOfOrigin.Owner.Character.CardPool);

        if (otherPools.Count == 0)
            return;

        IEnumerable<CardModel> cards = otherPools
            .SelectMany(pool => pool.GetUnlockedCards(tomeOfOrigin.Owner.UnlockState, tomeOfOrigin.Owner.RunState.CardMultiplayerConstraint));

        List<CardModel> generated = CardFactory.GetDistinctForCombat(
            tomeOfOrigin.Owner, cards, 1, tomeOfOrigin.Owner.RunState.Rng.CombatCardGeneration).ToList();

        if (generated.Count == 0)
            return;

        tomeOfOrigin.Flash();

        foreach (CardModel card in generated)
            card.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardsToCombat(generated, PileType.Hand, tomeOfOrigin.Owner);
    }
}