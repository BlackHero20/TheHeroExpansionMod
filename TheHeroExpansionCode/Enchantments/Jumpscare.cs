using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Jumpscare : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card?.CombatState == null)
            return;

        if (cardPlay.Card.Enchantment != this)
            return;

        var doomed = cardPlay.Card.CombatState.HittableEnemies
            .Where(e => e.IsAlive && e.GetPowerAmount<DoomPower>() >= e.CurrentHp)
            .ToList();

        if (doomed.Count == 0)
            return;
        
        await DoomPower.DoomKill(doomed);
    }
}