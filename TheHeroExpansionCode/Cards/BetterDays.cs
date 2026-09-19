using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class BetterDays() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon, TargetType.Self, nameof(BetterDays))
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1M),
        new PowerVar<BetterDaysPower>(1M),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BetterDays betterDays = this;
        await CreatureCmd.Damage(choiceContext, betterDays.Owner.Creature, betterDays.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, (CardModel) betterDays, cardPlay);
        await CardPileCmd.Draw(choiceContext, betterDays.DynamicVars.Cards.BaseValue, betterDays.Owner);
        await CreatureCmd.TriggerAnim(betterDays.Owner.Creature, "Cast", betterDays.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BetterDaysPower>(choiceContext, betterDays.Owner.Creature,
            betterDays.DynamicVars["BetterDaysPower"].BaseValue, betterDays.Owner.Creature, betterDays);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1M);
    }
}