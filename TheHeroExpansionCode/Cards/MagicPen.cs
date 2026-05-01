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

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class MagicPen() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Dreamy>();
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("Dreamy", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        MagicPen magicPen = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(magicPen.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, magicPen.Owner, prefs, (Func<CardModel, bool>) (card => card.Enchantment == null && (card.Type == CardType.Attack || card.Type == CardType.Skill)), (AbstractModel) magicPen)).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        CardCmd.Enchant<Dreamy>(card, magicPen.DynamicVars["Dreamy"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}