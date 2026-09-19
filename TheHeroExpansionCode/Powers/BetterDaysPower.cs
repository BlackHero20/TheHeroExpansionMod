using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class BetterDaysPower : TheHeroExpansionPower
{
    private const string _selfDamageKey = "SelfDamage";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        BetterDaysPower power = this;
        if (target != power.Owner)
            return;
        if (result.UnblockedDamage <= 0)
            return;
        if (power.Owner.CombatState.CurrentSide != power.Owner.Side)
            return;

        power.Flash();
        await CreatureCmd.Heal(power.Owner, (Decimal) result.UnblockedDamage);
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        BetterDaysPower power = this;
        if (side != this.Owner.Side)
            return;

        await PowerCmd.TickDownDuration((PowerModel) power);
    }
}