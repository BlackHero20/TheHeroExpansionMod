using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class AkumetsuPower : TheHeroExpansionPower
{
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
        AkumetsuPower power = this;
        if (target != power.Owner) return;
        if (result.UnblockedDamage <= 0) return;

        await PowerCmd.Remove(power);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        AkumetsuPower power = this;
        if (!participants.Contains(power.Owner)) return;

        await PowerCmd.Apply<AstralPower>(
            new ThrowingPlayerChoiceContext(),
            power.Owner, (decimal) power.Amount, power.Owner, null);

        await PowerCmd.Remove(power);
    }
}