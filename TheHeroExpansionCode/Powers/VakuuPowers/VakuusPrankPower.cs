using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;
public class VakuusPrankPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        VakuusPrankPower vakuusPrankPower = this;
        if (vakuusPrankPower.Owner.Player.Creature.Side != side)
            return;
        vakuusPrankPower.Flash();
        await CreatureCmd.Damage(choiceContext, vakuusPrankPower.Owner, (Decimal) vakuusPrankPower.Amount, ValueProp.Unpowered, vakuusPrankPower.Owner, (CardModel) null);
        VfxCmd.PlayOnCreatureCenter(vakuusPrankPower.Owner, "vfx/vfx_attack_blunt");
        await PowerCmd.Remove((PowerModel) vakuusPrankPower);
    }
}