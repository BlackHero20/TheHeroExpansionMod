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
public class BetterDays() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BetterDaysPower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BetterDays betterDays = this;
        await CreatureCmd.TriggerAnim(betterDays.Owner.Creature, "Cast", betterDays.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BetterDaysPower>(choiceContext, betterDays.Owner.Creature,
            betterDays.DynamicVars["BetterDaysPower"].BaseValue, betterDays.Owner.Creature, betterDays);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}