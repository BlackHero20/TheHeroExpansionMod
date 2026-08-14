using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public class SuperFormPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        SuperFormPower superFormPower = this;
        if (!participants.Contains<Creature>(superFormPower.Owner))
            return;
        superFormPower.Flash();
        await PowerCmd.Apply<MomentumPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), superFormPower.Owner, (Decimal) superFormPower.Amount, superFormPower.Owner, (CardModel) null);
    }
}