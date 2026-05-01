using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;
public sealed class HubrisPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        HubrisPower power = this;
        if (side != power.Owner.Side)
            return;

        if (power.Amount > 1)
        {
            await PowerCmd.Decrement((PowerModel)power);
        }
        else
        {
            power.Flash();
            await PowerCmd.Remove((PowerModel)power);
            await CreatureCmd.Kill(power.Owner);
        }
    }
}