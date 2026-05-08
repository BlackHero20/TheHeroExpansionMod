using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public class Advantageous : TheHeroExpansionEnchantment
{
    public override bool HasExtraCardText => true;
    
    public override bool ShowAmount => true;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Advantageous advantageous = this;
        if (advantageous.Status != EnchantmentStatus.Normal)
            return;
        await PlayerCmd.GainEnergy((Decimal) advantageous.Amount, advantageous.Card.Owner);
        advantageous.Status = EnchantmentStatus.Disabled;
    }
}