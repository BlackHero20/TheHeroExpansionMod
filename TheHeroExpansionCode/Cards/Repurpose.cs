using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(SilentCardPool))]
public class Repurpose() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Advantageous>((int)this.DynamicVars["Advantageous"].BaseValue);
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Advantageous", 2)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        Repurpose repurpose = this;
        RepurposePower power = await PowerCmd.Apply<RepurposePower>(choiceContext, repurpose.Owner.Creature, 1M, repurpose.Owner.Creature, repurpose); power.SetAdvantageous(repurpose.DynamicVars["Advantageous"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["Advantageous"].UpgradeValueBy(1);
    }
}