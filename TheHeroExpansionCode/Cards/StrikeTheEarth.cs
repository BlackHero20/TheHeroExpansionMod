using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class StrikeTheEarth() : TheHeroExpansionCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(6M, ValueProp.Move),
        new DynamicVar("Royal", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Royal>();
    
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Strike];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        StrikeTheEarth strikeTheEarth = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(strikeTheEarth.DynamicVars.Damage.BaseValue)
            .FromCard(strikeTheEarth, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        CardSelectorPrefs prefs = new CardSelectorPrefs(strikeTheEarth.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, strikeTheEarth.Owner, prefs, (Func<CardModel, bool>) (card => (card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power) && card.Enchantment == null), (AbstractModel) strikeTheEarth)).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        CardCmd.Enchant<Royal>(card, strikeTheEarth.DynamicVars["Royal"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3M);
    }
}