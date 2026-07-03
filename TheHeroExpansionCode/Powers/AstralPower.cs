using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class AstralPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay cardPlay)
    {
        if (dealer != Owner && !Owner.Pets.Contains(dealer)) return 1M;
        if (!props.IsPoweredAttack()) return 1M;
        if (cardSource == null) return 1M;
        return 2M;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AstralPower power = this;
        if (cardPlay.Card.Owner != power.Owner.Player) return;
        if (cardPlay.Card.Type != CardType.Attack) return;

        await PowerCmd.Decrement(power);
    }
}