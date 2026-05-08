using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public class Ravenous : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Ravenous ravenous = this;
        var state = ravenous.Card.Owner.PlayerCombatState;
        if (state?.OrbQueue == null || state.OrbQueue.Orbs.Count == 0)
            return;
        await OrbCmd.EvokeLast(choiceContext, ravenous.Card.Owner);
    }
}