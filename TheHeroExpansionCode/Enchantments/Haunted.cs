using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public class Haunted : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Haunted haunted = this;
        var owner = haunted.Card.Owner;
        var combatState = owner.Creature.CombatState;
        if (combatState == null) return;

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(
            Soul.Create(owner, 1, combatState).ToList(),
            PileType.Draw, owner, CardPilePosition.Random));
    }
}