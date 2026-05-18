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
    CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(3M),
        new ExtraDamageVar(3M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
            CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>()
                .Count(e => e.CardPlay.Card.Owner == card.Owner &&
                            e.CardPlay.Card.Tags.Contains(CardTag.Strike)))
    ];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SixHundredStrike sixHundredStrike = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(sixHundredStrike.DynamicVars.CalculatedDamage)
            .FromCard(sixHundredStrike)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.CalculationBase.UpgradeValueBy(1M);
        this.DynamicVars.ExtraDamage.UpgradeValueBy(1M);
    }
}