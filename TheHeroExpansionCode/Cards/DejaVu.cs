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
public class DejaVu() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Event,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.ReplayStatic)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DejaVu dejaVu = this;
        IEnumerable<CardModel> dejuVuTarget = await CardSelectCmd.FromHand(choiceContext, dejaVu.Owner, new CardSelectorPrefs(dejaVu.SelectionScreenPrompt, 1), (Func<CardModel, bool>) null, (AbstractModel) dejaVu);
        await CreatureCmd.TriggerAnim(dejaVu.Owner.Creature, "Cast", dejaVu.Owner.Character.CastAnimDelay);
        foreach (CardModel cardModel in dejuVuTarget)
        {
            ++cardModel.BaseReplayCount;
        }
        dejuVuTarget = (IEnumerable<CardModel>) null;
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}