using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class TheTaintedBlade() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Event,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<SilentCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6M, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<TaintedBladePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheTaintedBlade theTaintedBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        AttackCommand attackCommand = await DamageCmd.Attack(theTaintedBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theTaintedBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        int damageDealt = attackCommand.Results
            .SelectMany(r => r)
            .Sum(r => r.TotalDamage);

        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target,
            (decimal)damageDealt, theTaintedBlade.Owner.Creature, theTaintedBlade);

        await PowerCmd.Apply<TaintedBladePower>(choiceContext, cardPlay.Target,
            (decimal)damageDealt, theTaintedBlade.Owner.Creature, theTaintedBlade);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
    }
}