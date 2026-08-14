using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class KaraCancel() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self, nameof(KaraCancel))
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(2M),
        new CalculatedVar("Cards").WithMultiplier((card, _) =>
        {
            CardPlayFinishedEntry lastPlay = CombatManager.Instance.History.Entries
                .OfType<CardPlayFinishedEntry>()
                .LastOrDefault(e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card != card);
            return lastPlay == null ? 0M : (decimal)lastPlay.CardPlay.Resources.EnergyValue;
        })
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        KaraCancel karaCancel = this;
        decimal drawAmount = ((CalculatedVar)karaCancel.DynamicVars["Cards"]).Calculate(null);
        if (drawAmount <= 0) return;
        await CardPileCmd.Draw(choiceContext, drawAmount, karaCancel.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.CalculationBase.UpgradeValueBy(1M);
    }
}