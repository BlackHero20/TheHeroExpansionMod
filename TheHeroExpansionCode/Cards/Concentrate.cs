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

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class Concentrate() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Event,
    TargetType.Self)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<SilentCardPool>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Concentrate concentrate = this;
        await CardCmd.Discard(choiceContext,
            await CardSelectCmd.FromHandForDiscard(choiceContext, concentrate.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, concentrate.DynamicVars.Cards.IntValue),
                (Func<CardModel, bool>)null, (AbstractModel)concentrate));
        await PlayerCmd.GainEnergy(concentrate.DynamicVars.Energy.BaseValue, concentrate.Owner);
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Energy.UpgradeValueBy(1);
    }
}