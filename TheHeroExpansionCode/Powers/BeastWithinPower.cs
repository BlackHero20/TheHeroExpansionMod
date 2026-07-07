using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class BeastWithinPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    
    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        BeastWithinPower power = this;

        if (dealer != power.Owner)
            return;
        if (!props.IsPoweredAttack())
            return;
        if (!target.HasPower<VulnerablePower>())
            return;
        if (result.TotalDamage <= 0)
            return;
        if (cardSource?.TargetType is not (TargetType.AnyEnemy or TargetType.RandomEnemy))
            return;

        List<Creature> otherEnemies = power.CombatState
            .GetTeammatesOf(target)
            .Except([target])
            .Where(e => e.IsHittable)
            .ToList();

        if (otherEnemies.Count == 0)
            return;

        for (int i = 0; i < power.Amount; i++)
        {
            await CreatureCmd.Damage(
                choiceContext,
                (IEnumerable<Creature>)otherEnemies,
                (decimal)(result.TotalDamage + result.OverkillDamage),
                ValueProp.Unpowered | ValueProp.Move,
                power.Owner,
                null,
                null);
        }
    }
}