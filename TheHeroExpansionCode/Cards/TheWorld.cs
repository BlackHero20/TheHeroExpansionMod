using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class TheWorld() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override int CanonicalStarCost => 10;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<TheWorldPower>(1M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        TheWorld theWorld = this;
        await PowerCmd.Apply<TheWorldPower>(choiceContext, theWorld.Owner.Creature, theWorld.DynamicVars["TheWorldPower"].BaseValue, theWorld.Owner.Creature, (CardModel) theWorld);
    }

    protected override void OnUpgrade()
    {
        // Example upgrade: reduce cost
        this.EnergyCost.UpgradeBy(-1);
    }
}