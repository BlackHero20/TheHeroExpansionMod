using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class BeastWithin() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromPower<VulnerablePower>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<BeastWithinPower>(1M),
        new PowerVar<VulnerablePower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BeastWithin beastWithin = this;
        
        foreach (Creature enemy in (IEnumerable<Creature>)beastWithin.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, beastWithin.DynamicVars.Vulnerable.BaseValue, beastWithin.Owner.Creature, beastWithin);
        }
        
        await PowerCmd.Apply<BeastWithinPower>(choiceContext, beastWithin.Owner.Creature, beastWithin.DynamicVars["BeastWithinPower"].BaseValue, beastWithin.Owner.Creature, beastWithin);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Vulnerable.UpgradeValueBy(1M);
    }
}