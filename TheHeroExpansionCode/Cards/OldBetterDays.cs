using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class OldBetterDays() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self, nameof(OldBetterDays))
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OldBetterDaysPower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        OldBetterDays betterDays = this;
        await CreatureCmd.TriggerAnim(betterDays.Owner.Creature, "Cast", betterDays.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<OldBetterDaysPower>(choiceContext, betterDays.Owner.Creature,
            betterDays.DynamicVars["OldBetterDaysPower"].BaseValue, betterDays.Owner.Creature, betterDays);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}