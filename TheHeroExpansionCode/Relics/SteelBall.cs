using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;

[Pool(typeof(EventRelicPool))]
public class SteelBall() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<GoldenRatio>();

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("GoldenRatio", 1)
    ];

    public override async Task AfterObtained()
    {
        SteelBall steelBall = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(steelBall.Owner, (EnchantmentModel) ModelDb.Enchantment<GoldenRatio>(), 1, prefs))
        {
            CardCmd.Enchant<Infected>(card, steelBall.DynamicVars["GoldenRatio"].BaseValue);
            CardCmd.Preview(card);
        }
    }
}