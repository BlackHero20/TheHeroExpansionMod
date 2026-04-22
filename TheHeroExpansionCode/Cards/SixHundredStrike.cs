using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class SixHundredStrike() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    // as of now broken, scales on every attack instead of only strikes. will fix later
    private const string _calculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3M, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => 1 + CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Count((Func<CardPlayFinishedEntry, bool>) (e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card.Tags.Contains<CardTag>(CardTag.Strike)))))
    ];
    
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Strike];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        SixHundredStrike sixHundredStrike = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(sixHundredStrike.DynamicVars.Damage.BaseValue)
            .FromCard((CardModel)sixHundredStrike)
            .Targeting(cardPlay.Target)
            .WithHitCount((int)((CalculatedVar)sixHundredStrike.DynamicVars[_calculatedHitsKey]).Calculate(cardPlay.Target))
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(1M);
    }
}