using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class InstantAnima() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Haunted>();

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        InstantAnima instantAnima = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(instantAnima.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, instantAnima.Owner, prefs,
            card => card.Enchantment == null && card.EnergyCost.Canonical > 0 &&
                    (card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power),
            (AbstractModel)instantAnima)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Haunted>(card, 1M);
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}