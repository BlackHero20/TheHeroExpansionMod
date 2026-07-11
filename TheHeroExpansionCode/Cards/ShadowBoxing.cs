using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(DefectCardPool))]
public class ShadowBoxing() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new DynamicVar("Ravenous", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Ravenous>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ShadowBoxing shadowBoxing = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(shadowBoxing.DynamicVars.Damage.BaseValue)
            .FromCard(shadowBoxing, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        CardSelectorPrefs prefs = new CardSelectorPrefs(shadowBoxing.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, shadowBoxing.Owner, prefs,
            card => card.Enchantment == null &&
                    (card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power),
            (AbstractModel)shadowBoxing)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Ravenous>(card, shadowBoxing.DynamicVars["Ravenous"].BaseValue);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}