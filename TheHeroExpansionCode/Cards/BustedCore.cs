using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(TokenCardPool))]
public class BustedCore() : TheHeroExpansionCard(0,
    CardType.Status, CardRarity.Status,
    TargetType.Self)
{
    public override int MaxUpgradeLevel => 0;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("OrbSlots", 1M),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BustedCore bustedCore = this;
        await CreatureCmd.TriggerAnim(bustedCore.Owner.Creature, "PowerUp", bustedCore.Owner.Character.PowerUpAnimDelay);
        OrbCmd.RemoveSlots(Owner, DynamicVars["OrbSlots"].IntValue);
        await CardPileCmd.Draw(choiceContext, bustedCore.DynamicVars.Cards.BaseValue, bustedCore.Owner);
    }
}