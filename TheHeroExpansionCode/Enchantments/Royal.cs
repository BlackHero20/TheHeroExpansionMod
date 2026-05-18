using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public class Royal : TheHeroExpansionEnchantment
{
    public override bool HasExtraCardText => true;
    
    public override bool IsStackable => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StarsVar(1)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Royal royal = this;
        await PlayerCmd.GainStars(royal.DynamicVars.Stars.BaseValue, royal.Card.Owner);
    }
}